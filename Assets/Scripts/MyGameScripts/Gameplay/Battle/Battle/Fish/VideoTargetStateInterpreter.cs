using Fish;

namespace AppDto
{
    //using vistor pattern to calculate battle play controler
    public partial class VideoTargetState
    {
        public virtual IBattlePlayCtl Interprete()
        {
            return null;
        }
    }

    public partial class VideoActionTargetState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoBuffAddTargetState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoBuffRemoveTargetState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoDodgeTargetState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoDrivingTargetState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoRetreatState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoSoldierSwtichState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }

    public partial class VideoTargetExceptionState
    {
        public override IBattlePlayCtl Interprete()
        {
            return base.Interprete();
        }
    }
}