// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  UnitWaitingTrigger.cs
// Author   : willson
// Created  : 2014/12/23 
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;

public class UnitWaitingTrigger
{
    private HeroView _heroView;
    private readonly List<TriggerNpcUnit> _units;

    public UnitWaitingTrigger()
    {
        _units = new List<TriggerNpcUnit>();
    }

    public void SetHeroPlayer(HeroView heroView)
    {
        _heroView = heroView;
    }

    public void AddTriggerUnit(TriggerNpcUnit unit)
    {
        if (_units == null || unit == null)
            return;

        _units.Add(unit);
        unit.SetupHeroView(_heroView);
    }

    public void RemoveTriggerUnit(TriggerNpcUnit unit)
    {
        if (_units == null || unit == null)
            return;

        _units.Remove(unit);
    }

    public void Tick()
    {
        for (int i = 0, len = _units.Count; i < len; i++)
        {
            var unit = _units[i];
            if (unit.enabled == false)
            {
                continue;
            }

            //unit.FaceToHero();
            /*
			if (MissionGuidePathFinder.DataMgr.GetNextNpc() != null && MissionGuidePathFinder.DataMgr.GetNextNpc() != unit.GetNpc())
			{
				continue;
			}
			*/
            if (unit.touch)
            {
                unit.Trigger();

                if (unit.enabled == false)
                {
                    Stop();
                }
                break;
            }

            if (unit.waitingTrigger)
            {
                var direction = unit.GetPos() - _heroView.cachedTransform.position;
                if (direction.magnitude < 2)
                {
                    if (unit.NeedClose() == false)
                    {
                        _heroView.StopAndIdle();
                    }
                    unit.touch = true;
                }
            }

            if (unit.walk)
            {
                unit.UpdateWalk();
            }
        }
    }

    public void Stop()
    {
    }

    public void Play()
    {
        for (int i = 0, len = _units.Count; i < len; i++)
        {
            var unit = _units[i];
            unit.Reset();
        }
    }

    public void Reset()
    {
        for (int i = 0, len = _units.Count; i < len; i++)
        {
            var unit = _units[i];
            unit.Reset();
        }
    }

    public void Destroy()
    {
        Stop();
        _heroView = null;
    }
}