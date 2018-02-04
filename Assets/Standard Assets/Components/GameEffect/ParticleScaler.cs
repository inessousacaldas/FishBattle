//This script will only work in editor mode. You cannot adjust the scale dynamically in-game!

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

//[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour
{
    private ParticleAnimator[] _particleAnimators;
    private ParticleEmitter[] _particleEmitters;

    private ParticleSystem[] _particleSystems;
    private TrailRenderer[] _trailRenderers;
    public float particleScale = 1.0f;
    public bool alsoScaleGameobject = true;

    private float prevParticleScale = 1.0f;

    private void Start()
    {
        // init particle system
        if (_particleSystems == null)
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (var system in _particleSystems)
        {
            system.Clear();
        }
    }

    public void SetScale(float scale)
    {
        particleScale = scale;
    }

    public void Reset()
    {
        prevParticleScale = 1.0f;
        particleScale = 1.0f;
        alsoScaleGameobject = true;

        Start();
    }

    private void Update()
    {
        //check if we need to update
        if (particleScale > 0 && Math.Abs(prevParticleScale - particleScale) > 0.01f)
        {
            if (alsoScaleGameobject)
                transform.localScale = new Vector3(particleScale, particleScale, particleScale);

            float scaleFactor = particleScale / prevParticleScale;

            //scale legacy particle systems
            ScaleLegacySystems(scaleFactor);

            //scale shuriken particle systems
            ScaleShurikenSystems(scaleFactor);

            //scale trail renders
            ScaleTrailRenderers(scaleFactor);

            prevParticleScale = particleScale;
        }
    }

    private void ScaleShurikenSystems(float scaleFactor)
    {
        //get all shuriken systems we need to do scaling on
        if (_particleSystems == null)
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);

        foreach (var ps in _particleSystems)
        {
            ps.startSpeed *= scaleFactor;
            ps.startSize *= scaleFactor;
            ps.gravityModifier *= scaleFactor;

#if UNITY_EDITOR
            //some variables cannot be accessed through regular script, we will acces them through a serialized object
            var so = new SerializedObject(ps);

            //unity 4.0 and onwards will already do this one for us
            so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
            so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
            so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
            so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
            so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;

            so.ApplyModifiedProperties();
#endif
        }
    }

    private void ScaleLegacySystems(float scaleFactor)
    {
        //get all emitters we need to do scaling on
        if (_particleEmitters == null)
            _particleEmitters = GetComponentsInChildren<ParticleEmitter>(true);

        //apply scaling to emitters
        foreach (var emitter in _particleEmitters)
        {
            emitter.minSize *= scaleFactor;
            emitter.maxSize *= scaleFactor;
            emitter.worldVelocity *= scaleFactor;
            emitter.localVelocity *= scaleFactor;
            emitter.rndVelocity *= scaleFactor;

#if UNITY_EDITOR
            //some variables cannot be accessed through regular script, we will acces them through a serialized object
            var so = new SerializedObject(emitter);

            so.FindProperty("m_Ellipsoid").vector3Value *= scaleFactor;
            so.FindProperty("tangentVelocity").vector3Value *= scaleFactor;
            so.ApplyModifiedProperties();
#endif
        }

        //get all animators we need to do scaling on
        if (_particleAnimators == null)
            _particleAnimators = GetComponentsInChildren<ParticleAnimator>(true);

        //apply scaling to animators
        foreach (var animator in _particleAnimators)
        {
            animator.force *= scaleFactor;
            animator.rndForce *= scaleFactor;
        }
    }

    private void ScaleTrailRenderers(float scaleFactor)
    {
        //get all animators we need to do scaling on
        if (_trailRenderers == null)
            _trailRenderers = GetComponentsInChildren<TrailRenderer>(true);

        //apply scaling to animators
        foreach (var trail in _trailRenderers)
        {
            trail.startWidth *= scaleFactor;
            trail.endWidth *= scaleFactor;
        }
    }
}