using System;
using AppDto;

namespace SkillEditor
{
    public interface IBattleEditorPreviewController
    {
        void OpenPreview(int pEnemyCount,int pEnemyFormation);

        void ReplayPreview(int pSkillId);

        void ClosePreview(bool pCloseBgAlso);
    }
}

