using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace EditorNS
{
    public abstract class ArtSceneChecker
    {
        #region 场景工厂

        public static ArtSceneChecker CreateChecker(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name.Contains("final"))
            {
                return null;
            }
            else if (name.Contains("Scene"))
            {
                return new WorldSceneChecker(path);
            }
            else if (name.Contains("Battle"))
            {
                return new BattleSceneChecker(path);
            }

            return null;
        }

        #endregion


        protected string _scenePath;
        protected string _sceneName;
        protected Transform _rootTransform;

        public ArtSceneChecker(string scenePath)
        {
            _scenePath = scenePath;
            _sceneName = Path.GetFileNameWithoutExtension(_scenePath);
        }

        #region 一些输出的优化

        protected virtual string Log(string msg)
        {
            msg = string.Format("{0}\n{1}", msg, _scenePath);
            Debug.Log(msg);
            return msg;
        }

        protected virtual string LogWarning(string msg)
        {
            msg = string.Format("{0}\n{1}", msg, _scenePath);
            Debug.LogWarning(msg);
//            Debug.LogError(msg);
            return msg;
        }

        protected virtual string LogError(string msg)
        {
            msg = string.Format("{0}\n{1}", msg, _scenePath);
            Debug.LogError(msg);
            return msg;
        }

        #endregion


        public virtual void Check()
        {
            CheckBegin();
            Checking();
            CheckEnd();
        }


        protected virtual void CheckBegin()
        {
            if (EditorSceneManager.GetActiveScene().path != _scenePath)
            {
                EditorSceneManager.OpenScene(_scenePath);
            }
            Log(string.Format("开始检查：{0}！", _sceneName));
        }

        protected virtual void Checking()
        {
            CheckRootList();
            if (_rootTransform == null)
            {
                LogWarning("该场景不存在根节点！");
                return;
            }

            CheckChildren();
            CheckColliders();
        }

        protected virtual void CheckEnd()
        {
            Log(string.Format("结束检查：{0}！", _sceneName));
        }


        /// <summary>
        /// 遍历所有的根节点
        /// </summary>
        protected virtual void CheckRootList()
        {
            foreach (var go in EditorHelper.SceneRoots())
            {
                if (!CheckRoot(go))
                {
                    if (go.activeSelf)
                    {
                        LogError(string.Format("根节点{0}没有删掉，并且是开启的！", go));
                    }
                    else
                    {
                        LogWarning(string.Format("根节点{0}是多余的！", go));
                    }
                }
                else
                {
                    _rootTransform = go.transform;
                }
            }
        }


        protected virtual bool CheckRoot(GameObject go)
        {
            if (go.name == GetRootName())
            {
                if (!go.activeSelf)
                {
                    LogError(string.Format("根节点{0}必须激活！", go));
                }

                var trans = go.transform;
                if (trans.localPosition != Vector3.zero ||
                    trans.localEulerAngles != Vector3.zero ||
                    trans.localScale != Vector3.one)
                {
                    LogError(string.Format("根节点{0}没有归位！", go));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract string GetRootName();

        protected virtual void CheckChildren()
        {
            foreach (Transform child in _rootTransform)
            {
                if (child.name.Contains(_sceneName))
                {
                    CheckScene(child);
                }
                else if (child.name.Contains("light"))
                {
                    CheckLight(child);
                }
                else if (child.name == "way")
                {
                    CheckWay(child);
                }
                else if (child.name == "way_all")
                {
                    CheckWayAll(child);
                }
                else if (child.name.Contains("Effect"))
                {
                    CheckEffect(child);
                }
            }
        }


        protected virtual void CheckScene(Transform scene)
        {
            if (!scene.gameObject.activeSelf)
            {
                LogError(string.Format("没有打开场景实体：{0}！", scene));
            }
        }

        protected virtual void CheckLight(Transform light)
        {
            if (light.gameObject.activeSelf)
            {
                LogError(string.Format("灯光{0}没有关闭！", light));
            }
        }

        protected virtual void CheckWay(Transform way)
        {
            if (way.gameObject.activeSelf)
            {
                LogError(string.Format("way：{0}没有关闭！", way));
            }
        }


        protected virtual void CheckWayAll(Transform wayAll)
        {
            if (!wayAll.gameObject.activeSelf)
            {
                LogError(string.Format("way_all：{0}没有打开！", wayAll));
            }
        }


        protected virtual void CheckEffect(Transform effect)
        {
            if (!effect.gameObject.activeSelf)
            {
                LogError(string.Format("没有打开特效实体：{0}！", effect));
            }

            if (!effect.gameObject.CompareTag("SceneEffect"))
            {
                LogError(string.Format("特效{0}没有设置tag SceneEffect", effect));
            }
        }


        protected virtual void CheckColliders()
        {
            foreach (var collider in _rootTransform.GetComponentsInChildren<Collider>(true))
            {
                if (collider.name != "way" && collider.name != "way_all")
                {
                    if (collider.gameObject.activeInHierarchy)
                    {
                        LogError(string.Format("多余的Collider：{0}，且没有关闭！", collider));
                    }
                    else
                    {
                        LogWarning(string.Format("多余的Collider：{0}！", collider));
                    }
                }
            }
        }
    }
}