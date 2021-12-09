// Simple Side-Menu - https://assetstore.unity.com/packages/tools/gui/simple-side-menu-143623
// Copyright (c) Daniel Lochner

using UnityEditor;

namespace DanielLochner.Assets.SimpleSideMenu
{
    [CustomEditor(typeof(object), true)]
    [CanEditMultipleObjects]
    public class SSMCopyrightEditor : CopyrightEditor
    {
        public override string Product => "Simple Side-Menu";
        public override string CopyrightHolder => "Daniel Lochner";
    }
}