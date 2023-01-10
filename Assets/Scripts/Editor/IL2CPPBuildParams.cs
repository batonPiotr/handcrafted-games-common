namespace HandcraftedGames.Common
{
    using UnityEngine;
    using UnityEditor;

    public class IL2CPPBuildParams
    {

        /// <summary>
        /// https://forum.unity.com/threads/android-builds-failing-when-script-debugging-is-enabled.1027357/
        /// Android builds failing when 'Script Debugging' is enabled
        ///
        /// And: https://issuetracker.unity3d.com/issues/il2cpp-executionengineexception-error-is-being-spammed-in-build-when-calling-linq-dot-parallelenumerable-dot-forall
        /// </summary>
        [InitializeOnLoadMethod]
        static void FixScriptDebuggingBuildFail()
        {
            Debug.Log("Fixing il2cpp\nSetting --generic-virtual-method-iterations=2");
            PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=2"); // default is 1
        }
    }
}