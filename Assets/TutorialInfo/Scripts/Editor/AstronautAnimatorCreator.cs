using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class AstronautAnimatorCreator
{
    [MenuItem("Tools/Animator/Create Astronaut Controller From Selection")]
    public static void CreateFromSelection()
    {
        var motions = Selection.objects.OfType<Motion>().ToList();
        var clips = Selection.objects.OfType<AnimationClip>().Cast<Motion>().ToList();
        motions.AddRange(clips);
        Motion idle = Find(motions, new[] { "idle", "stand", "standing" });
        Motion walk = Find(motions, new[] { "walk", "walking" });
        Motion run = Find(motions, new[] { "run", "running" });
        Motion push = Find(motions, new[] { "push" });
        Motion attack = Find(motions, new[] { "attack", "punch", "fight" });
        Motion preJump = Find(motions, new[] { "prejump", "pre-jump", "pre_jump" });
        Motion jumping = Find(motions, new[] { "jumping" });
        Motion jump = Find(motions, new[] { "jump" });
        Motion landing = Find(motions, new[] { "land", "landing" });

        string dir = "Assets/TutorialInfo/Animations";
        if (!AssetDatabase.IsValidFolder(dir))
        {
            var parts = dir.Split('/');
            string path = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = Path.Combine(path, parts[i]).Replace("\\", "/");
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(path, parts[i]);
                path = next;
            }
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, "AstronautController.controller"));
        var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsPushing", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;

        var blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, useAutomaticThresholds = false, blendParameter = "Speed" };
        AssetDatabase.AddObjectToAsset(blendTree, controller);
        var idleM = idle ?? walk ?? run ?? attack ?? jump ?? landing;
        if (idleM != null) blendTree.AddChild(idleM, 0f);
        if (walk != null) blendTree.AddChild(walk, 2.5f); else if (idleM != null) blendTree.AddChild(idleM, 2.5f);
        if (run != null) blendTree.AddChild(run, 6f);

        var locomotion = sm.AddState("Locomotion");
        locomotion.motion = blendTree;
        sm.defaultState = locomotion;

        AnimatorState pushState = null;
        if (push != null)
        {
            pushState = sm.AddState("Push");
            pushState.motion = push;
            var toPush = locomotion.AddTransition(pushState);
            toPush.hasExitTime = false;
            toPush.AddCondition(AnimatorConditionMode.If, 0f, "IsPushing");
            var fromPush = pushState.AddTransition(locomotion);
            fromPush.hasExitTime = false;
            fromPush.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsPushing");
        }

        AnimatorState attackState = null;
        if (attack != null)
        {
            attackState = sm.AddState("Attack");
            attackState.motion = attack;
            var anyToAttack = sm.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            var backFromAttack = attackState.AddTransition(locomotion);
            backFromAttack.hasExitTime = true;
            backFromAttack.exitTime = 0.9f;
        }

        AnimatorState preJumpState = null;
        AnimatorState jumpingState = null;
        AnimatorState landingState = null;

        if (preJump != null)
        {
            preJumpState = sm.AddState("PreJump");
            preJumpState.motion = preJump;
            var toPre = locomotion.AddTransition(preJumpState);
            toPre.hasExitTime = false;
            toPre.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        }

        if (jumping != null || jump != null)
        {
            jumpingState = sm.AddState("Jumping");
            jumpingState.motion = jumping ?? jump;
            var fromPreToJump = preJumpState != null ? preJumpState.AddTransition(jumpingState) : locomotion.AddTransition(jumpingState);
            fromPreToJump.hasExitTime = true;
            fromPreToJump.exitTime = 0.8f;
        }

        if (landing != null)
        {
            landingState = sm.AddState("Landing");
            landingState.motion = landing;
            if (jumpingState != null)
            {
                var toLanding = jumpingState.AddTransition(landingState);
                toLanding.hasExitTime = false;
                toLanding.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            }
            var toLocomotion = landingState.AddTransition(locomotion);
            toLocomotion.hasExitTime = false;
            toLocomotion.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animator Created", "Controller creado en:\n" + assetPath + "\nAsigna este Controller al componente Animator del objeto 'astronauta'.", "OK");
    }

    [MenuItem("Tools/Animator/Create Astronaut Controller From Folder")]    
    public static void CreateFromFolder()
    {
        string searchFolder = "Assets/astronauta/animaciones";
        var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { searchFolder });
        var motions = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).Select(p => AssetDatabase.LoadAssetAtPath<AnimationClip>(p)).Cast<Motion>().ToList();

        Motion idle = Find(motions, new[] { "idle", "stand", "standing" });
        Motion walk = Find(motions, new[] { "walk", "walking" });
        Motion run = Find(motions, new[] { "run", "running" });
        Motion push = Find(motions, new[] { "push" });
        Motion attack = Find(motions, new[] { "attack", "punch", "fight" });
        Motion preJump = Find(motions, new[] { "prejump", "pre-jump", "pre_jump" });
        Motion jumping = Find(motions, new[] { "jumping" });
        Motion jump = Find(motions, new[] { "jump" });
        Motion landing = Find(motions, new[] { "land", "landing" });

        string dir = searchFolder;
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, "AstronautController_Auto.controller"));
        var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsPushing", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;

        var blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, useAutomaticThresholds = false, blendParameter = "Speed" };
        AssetDatabase.AddObjectToAsset(blendTree, controller);
        var idleM = idle ?? walk ?? run ?? attack ?? jump ?? landing;
        if (idleM != null) blendTree.AddChild(idleM, 0f);
        if (walk != null) blendTree.AddChild(walk, 2.5f); else if (idleM != null) blendTree.AddChild(idleM, 2.5f);
        if (run != null) blendTree.AddChild(run, 6f);

        var locomotion = sm.AddState("Locomotion");
        locomotion.motion = blendTree;
        sm.defaultState = locomotion;

        if (push != null)
        {
            var pushState = sm.AddState("Push");
            pushState.motion = push;
            var toPush = locomotion.AddTransition(pushState);
            toPush.hasExitTime = false;
            toPush.AddCondition(AnimatorConditionMode.If, 0f, "IsPushing");
            var fromPush = pushState.AddTransition(locomotion);
            fromPush.hasExitTime = false;
            fromPush.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsPushing");
        }

        if (attack != null)
        {
            var attackState = sm.AddState("Attack");
            attackState.motion = attack;
            var anyToAttack = sm.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            var backFromAttack = attackState.AddTransition(locomotion);
            backFromAttack.hasExitTime = true;
            backFromAttack.exitTime = 0.9f;
        }

        AnimatorState preJumpState = null;
        AnimatorState jumpingState = null;
        AnimatorState landingState = null;

        if (preJump != null)
        {
            preJumpState = sm.AddState("PreJump");
            preJumpState.motion = preJump;
            var toPre = locomotion.AddTransition(preJumpState);
            toPre.hasExitTime = false;
            toPre.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        }

        if (jumping != null || jump != null)
        {
            jumpingState = sm.AddState("Jumping");
            jumpingState.motion = jumping ?? jump;
            var fromPreToJump = preJumpState != null ? preJumpState.AddTransition(jumpingState) : locomotion.AddTransition(jumpingState);
            fromPreToJump.hasExitTime = true;
            fromPreToJump.exitTime = 0.8f;
        }

        if (landing != null)
        {
            landingState = sm.AddState("Landing");
            landingState.motion = landing;
            if (jumpingState != null)
            {
                var toLanding = jumpingState.AddTransition(landingState);
                toLanding.hasExitTime = false;
                toLanding.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            }
            var toLocomotion = landingState.AddTransition(locomotion);
            toLocomotion.hasExitTime = false;
            toLocomotion.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        }

        foreach (var m in new[] { idle, walk, run })
        {
            var clip = m as AnimationClip;
            if (clip == null) continue;
            var so = new SerializedObject(clip);
            var prop = so.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (prop != null) { prop.boolValue = true; so.ApplyModifiedProperties(); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animator Created", "Controller creado en:\n" + assetPath + "\nAsigna este Controller al componente Animator del objeto 'astronauta'.", "OK");
    }

    [MenuItem("Tools/Animator/Configure Selected Animator From Folder")]    
    public static void ConfigureSelectedFromFolder()
    {
        var controller = Selection.activeObject as AnimatorController;
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Selecciona un AnimatorController", "Selecciona en el Project el controller que quieres configurar y vuelve a ejecutar el menú.", "OK");
            return;
        }

        string searchFolder = "Assets/astronauta/animaciones";
        var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { searchFolder });
        var motions = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).Select(p => AssetDatabase.LoadAssetAtPath<AnimationClip>(p)).Cast<Motion>().ToList();

        Motion idle = Find(motions, new[] { "idle", "stand", "standing" });
        Motion walk = Find(motions, new[] { "walk", "walking" });
        Motion run = Find(motions, new[] { "run", "running" });
        Motion push = Find(motions, new[] { "push" });
        Motion attack = Find(motions, new[] { "attack", "punch", "fight" });
        Motion preJump = Find(motions, new[] { "prejump", "pre-jump", "pre_jump" });
        Motion jumping = Find(motions, new[] { "jumping" });
        Motion jump = Find(motions, new[] { "jump" });
        Motion landing = Find(motions, new[] { "land", "landing" });

        EnsureParam(controller, "Speed", AnimatorControllerParameterType.Float);
        EnsureParam(controller, "IsGrounded", AnimatorControllerParameterType.Bool);
        EnsureParam(controller, "IsRunning", AnimatorControllerParameterType.Bool);
        EnsureParam(controller, "IsPushing", AnimatorControllerParameterType.Bool);
        EnsureParam(controller, "Jump", AnimatorControllerParameterType.Trigger);
        EnsureParam(controller, "Attack", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;
        AnimatorState locomotion = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Locomotion");
        if (locomotion == null) locomotion = sm.AddState("Locomotion");
        sm.defaultState = locomotion;

        var blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, useAutomaticThresholds = false, blendParameter = "Speed" };
        AssetDatabase.AddObjectToAsset(blendTree, controller);
        var idleM = idle ?? walk ?? run ?? attack ?? jump ?? landing;
        if (idleM != null) blendTree.AddChild(idleM, 0f);
        if (walk != null) blendTree.AddChild(walk, 2.5f); else if (idleM != null) blendTree.AddChild(idleM, 2.5f);
        if (run != null) blendTree.AddChild(run, 6f);
        locomotion.motion = blendTree;

        if (push != null)
        {
            var pushState = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Push") ?? sm.AddState("Push");
            pushState.motion = push;
            var toPush = locomotion.AddTransition(pushState);
            toPush.hasExitTime = false;
            toPush.AddCondition(AnimatorConditionMode.If, 0f, "IsPushing");
            var fromPush = pushState.AddTransition(locomotion);
            fromPush.hasExitTime = false;
            fromPush.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsPushing");
        }

        if (attack != null)
        {
            var attackState = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Attack") ?? sm.AddState("Attack");
            attackState.motion = attack;
            var anyToAttack = sm.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            var backFromAttack = attackState.AddTransition(locomotion);
            backFromAttack.hasExitTime = true;
            backFromAttack.exitTime = 0.9f;
        }

        AnimatorState preJumpState = null;
        AnimatorState jumpingState = null;
        AnimatorState landingState = null;

        if (preJump != null)
        {
            preJumpState = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "PreJump") ?? sm.AddState("PreJump");
            preJumpState.motion = preJump;
            var toPre = locomotion.AddTransition(preJumpState);
            toPre.hasExitTime = false;
            toPre.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        }

        if (jumping != null || jump != null)
        {
            jumpingState = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Jumping") ?? sm.AddState("Jumping");
            jumpingState.motion = jumping ?? jump;
            var fromPreToJump = preJumpState != null ? preJumpState.AddTransition(jumpingState) : locomotion.AddTransition(jumpingState);
            fromPreToJump.hasExitTime = true;
            fromPreToJump.exitTime = 0.8f;
        }

        if (landing != null)
        {
            landingState = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Landing") ?? sm.AddState("Landing");
            landingState.motion = landing;
            if (jumpingState != null)
            {
                var toLanding = jumpingState.AddTransition(landingState);
                toLanding.hasExitTime = false;
                toLanding.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            }
            var toLocomotion = landingState.AddTransition(locomotion);
            toLocomotion.hasExitTime = false;
            toLocomotion.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        }

        foreach (var m in new[] { idle, walk, run })
        {
            var clip = m as AnimationClip;
            if (clip == null) continue;
            var so = new SerializedObject(clip);
            var prop = so.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (prop != null) { prop.boolValue = true; so.ApplyModifiedProperties(); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animator Configured", "Controller configurado desde la carpeta:\n" + searchFolder + "\nSi falta algún clip, puedes ajustarlo manualmente en el Animator.", "OK");
    }

    static void EnsureParam(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        if (!controller.parameters.Any(p => p.name == name)) controller.AddParameter(name, type);
    }

    [MenuItem("Tools/Animator/Build Astronaut Controller (Exact Names)")]
    public static void BuildExactFromFolder()
    {
        string searchFolder = "Assets/astronauta/animaciones";
        var motions = AssetDatabase
            .FindAssets("t:AnimationClip", new[] { searchFolder })
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<AnimationClip>(p))
            .Cast<Motion>()
            .ToList();

        Motion idle = FindExact(motions, new[] { "Standing", "Standing W", "Idle" });
        Motion walk = FindExact(motions, new[] { "Walking", "Walk" });
        Motion run = FindExact(motions, new[] { "Running", "Run" });
        Motion push = FindExact(motions, new[] { "Push Start", "Push" });
        Motion attack = FindExact(motions, new[] { "Fist Fight B", "Attack" });
        Motion preJump = FindExact(motions, new[] { "Pre-Jump", "PreJump" });
        Motion jumping = FindExact(motions, new[] { "Jumping" });
        Motion jump = FindExact(motions, new[] { "Jump" });
        Motion landing = FindExact(motions, new[] { "Landing", "Land" });

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(searchFolder, "AstronautController_Exact.controller"));
        var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsPushing", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;
        var locomotion = sm.AddState("Locomotion");
        sm.defaultState = locomotion;

        var blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, useAutomaticThresholds = false, blendParameter = "Speed" };
        AssetDatabase.AddObjectToAsset(blendTree, controller);
        var idleM = idle ?? walk ?? run ?? attack ?? jump ?? landing;
        if (idleM != null) blendTree.AddChild(idleM, 0f);
        if (walk != null) blendTree.AddChild(walk, 2.5f); else if (idleM != null) blendTree.AddChild(idleM, 2.5f);
        if (run != null) blendTree.AddChild(run, 6f);
        locomotion.motion = blendTree;

        if (push != null)
        {
            var pushState = sm.AddState("Push");
            pushState.motion = push;
            var toPush = locomotion.AddTransition(pushState);
            toPush.hasExitTime = false;
            toPush.AddCondition(AnimatorConditionMode.If, 0f, "IsPushing");
            var fromPush = pushState.AddTransition(locomotion);
            fromPush.hasExitTime = false;
            fromPush.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsPushing");
        }

        if (attack != null)
        {
            var attackState = sm.AddState("Attack");
            attackState.motion = attack;
            var anyToAttack = sm.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            var backFromAttack = attackState.AddTransition(locomotion);
            backFromAttack.hasExitTime = true;
            backFromAttack.exitTime = 0.9f;
        }

        AnimatorState preJumpState = null;
        AnimatorState jumpingState = null;
        AnimatorState landingState = null;

        if (preJump != null)
        {
            preJumpState = sm.AddState("PreJump");
            preJumpState.motion = preJump;
            var toPre = locomotion.AddTransition(preJumpState);
            toPre.hasExitTime = false;
            toPre.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        }

        if (jumping != null || jump != null)
        {
            jumpingState = sm.AddState("Jumping");
            jumpingState.motion = jumping ?? jump;
            var fromPreToJump = preJumpState != null ? preJumpState.AddTransition(jumpingState) : locomotion.AddTransition(jumpingState);
            fromPreToJump.hasExitTime = true;
            fromPreToJump.exitTime = 0.8f;
        }

        if (landing != null)
        {
            landingState = sm.AddState("Landing");
            landingState.motion = landing;
            if (jumpingState != null)
            {
                var toLanding = jumpingState.AddTransition(landingState);
                toLanding.hasExitTime = false;
                toLanding.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            }
            var toLocomotion = landingState.AddTransition(locomotion);
            toLocomotion.hasExitTime = false;
            toLocomotion.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        }

        foreach (var m in new[] { idle, walk, run })
        {
            var clip = m as AnimationClip;
            if (clip == null) continue;
            var so = new SerializedObject(clip);
            var prop = so.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (prop != null) { prop.boolValue = true; so.ApplyModifiedProperties(); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animator Created", "Controller creado en:\n" + assetPath + "\nAsigna este Controller al componente Animator del objeto 'astronauta'.", "OK");
    }

    static Motion FindExact(System.Collections.Generic.List<Motion> motions, string[] names)
    {
        foreach (var m in motions)
        {
            var n = m.name.ToLowerInvariant();
            if (names.Any(p => n == p.ToLowerInvariant() || n.StartsWith(p.ToLowerInvariant()))) return m;
        }
        return null;
    }

    static Motion LoadByNames(string[] names, string[] folders)
    {
        foreach (var name in names)
        {
            var guids = AssetDatabase.FindAssets($"t:AnimationClip name:{name}", folders);
            if (guids != null && guids.Length > 0)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (clip != null) return clip;
            }
        }
        return null;
    }

    [MenuItem("Tools/Animator/Build Astronaut Controller (Use Provided Names)")]
    public static void BuildProvidedNames()
    {
        string folder = "Assets/astronauta/animaciones";
        string[] folders = new[] { folder };

        Motion idle = LoadByNames(new[] { "Standing W", "Standing", "Idle" }, folders);
        Motion walk = LoadByNames(new[] { "Walking", "Walk" }, folders);
        Motion run = LoadByNames(new[] { "Running", "Run" }, folders);
        Motion push = LoadByNames(new[] { "Push Start", "Push" }, folders);
        Motion attack = LoadByNames(new[] { "Fist Fight B", "Attack" }, folders);
        Motion preJump = LoadByNames(new[] { "Pre-Jump", "PreJump" }, folders);
        Motion jumping = LoadByNames(new[] { "Jumping" }, folders);
        Motion jump = LoadByNames(new[] { "Jump" }, folders);
        Motion landing = LoadByNames(new[] { "Landing", "Land" }, folders);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(folder, "AstronautController_Provided.controller"));
        var controller = AnimatorController.CreateAnimatorControllerAtPath(assetPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsPushing", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var sm = controller.layers[0].stateMachine;

        var locomotion = sm.AddState("Locomotion");
        sm.defaultState = locomotion;

        var blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, useAutomaticThresholds = false, blendParameter = "Speed" };
        AssetDatabase.AddObjectToAsset(blendTree, controller);
        var idleM = idle ?? walk ?? run ?? attack ?? jump ?? landing;
        if (idleM != null) blendTree.AddChild(idleM, 0f);
        if (walk != null) blendTree.AddChild(walk, 2.5f); else if (idleM != null) blendTree.AddChild(idleM, 2.5f);
        if (run != null) blendTree.AddChild(run, 6f);
        locomotion.motion = blendTree;

        if (push != null)
        {
            var pushState = sm.AddState("Push");
            pushState.motion = push;
            var toPush = locomotion.AddTransition(pushState);
            toPush.hasExitTime = false;
            toPush.AddCondition(AnimatorConditionMode.If, 0f, "IsPushing");
            var fromPush = pushState.AddTransition(locomotion);
            fromPush.hasExitTime = false;
            fromPush.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsPushing");
        }

        if (attack != null)
        {
            var attackState = sm.AddState("Attack");
            attackState.motion = attack;
            var anyToAttack = sm.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0f, "Attack");
            var backFromAttack = attackState.AddTransition(locomotion);
            backFromAttack.hasExitTime = true;
            backFromAttack.exitTime = 0.9f;
        }

        AnimatorState preJumpState = null;
        AnimatorState jumpingState = null;
        AnimatorState landingState = null;

        if (preJump != null)
        {
            preJumpState = sm.AddState("PreJump");
            preJumpState.motion = preJump;
            var toPre = locomotion.AddTransition(preJumpState);
            toPre.hasExitTime = false;
            toPre.AddCondition(AnimatorConditionMode.If, 0f, "Jump");
        }

        if (jumping != null || jump != null)
        {
            jumpingState = sm.AddState("Jumping");
            jumpingState.motion = jumping ?? jump;
            var fromPreToJump = preJumpState != null ? preJumpState.AddTransition(jumpingState) : locomotion.AddTransition(jumpingState);
            fromPreToJump.hasExitTime = true;
            fromPreToJump.exitTime = 0.8f;
        }

        if (landing != null)
        {
            landingState = sm.AddState("Landing");
            landingState.motion = landing;
            if (jumpingState != null)
            {
                var toLanding = jumpingState.AddTransition(landingState);
                toLanding.hasExitTime = false;
                toLanding.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
            }
            var toLocomotion = landingState.AddTransition(locomotion);
            toLocomotion.hasExitTime = false;
            toLocomotion.AddCondition(AnimatorConditionMode.If, 0f, "IsGrounded");
        }

        foreach (var m in new[] { idle, walk, run })
        {
            var clip = m as AnimationClip;
            if (clip == null) continue;
            var so = new SerializedObject(clip);
            var prop = so.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (prop != null) { prop.boolValue = true; so.ApplyModifiedProperties(); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Animator Created", "Controller creado en:\n" + assetPath + "\nAsigna este Controller al componente Animator del objeto 'astronauta'.", "OK");
    }
    static Motion Find(System.Collections.Generic.List<Motion> motions, string[] patterns)
    {
        foreach (var m in motions)
        {
            var n = m.name.ToLowerInvariant();
            if (patterns.Any(p => n.Contains(p))) return m;
        }
        return null;
    }

    [MenuItem("Tools/Animator/Assign Locomotion From Child (Standing/Walking/Running)")]
    public static void AssignLocomotionFromChild()
    {
        var go = Selection.activeGameObject;
        if (go == null) { EditorUtility.DisplayDialog("Selecciona el objeto", "Selecciona el hijo 'astronauta' en la jerarquía.", "OK"); return; }
        var animator = go.GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null) { EditorUtility.DisplayDialog("Sin Animator", "El objeto seleccionado no tiene Animator o Controller.", "OK"); return; }
        var controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null) { EditorUtility.DisplayDialog("Controller incompatible", "El Animator usa un controller no editable.", "OK"); return; }

        string searchFolder = "Assets/astronauta/animaciones";
        var motions = AssetDatabase
            .FindAssets("t:AnimationClip", new[] { searchFolder })
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<AnimationClip>(p))
            .Cast<Motion>()
            .ToList();

        Motion idle = FindExact(motions, new[] { "Standing W", "Standing", "Idle" });
        Motion walk = FindExact(motions, new[] { "Walking", "Walk" });
        Motion run = FindExact(motions, new[] { "Running", "Run" });

        var sm = controller.layers[0].stateMachine;
        var locomotion = sm.states.Select(s => s.state).FirstOrDefault(s => s.name == "Locomotion") ?? sm.AddState("Locomotion");
        sm.defaultState = locomotion;

        var blendTree = locomotion.motion as BlendTree;
        if (blendTree == null)
        {
            blendTree = new BlendTree { name = "Locomotion", blendType = BlendTreeType.Simple1D, blendParameter = "Speed", useAutomaticThresholds = false };
            AssetDatabase.AddObjectToAsset(blendTree, controller);
            locomotion.motion = blendTree;
        }
        blendTree.blendParameter = "Speed";
        blendTree.useAutomaticThresholds = false;
        var children = blendTree.children.ToList();
        children.Clear();
        if (idle != null) children.Add(new ChildMotion { motion = idle, threshold = 0f });
        if (walk != null) children.Add(new ChildMotion { motion = walk, threshold = 0.2835f });
        if (run != null) children.Add(new ChildMotion { motion = run, threshold = 1f });
        blendTree.children = children.ToArray();

        foreach (var m in new[] { idle, walk, run })
        {
            var clip = m as AnimationClip; if (clip == null) continue;
            var so = new SerializedObject(clip);
            var loop = so.FindProperty("m_AnimationClipSettings.m_LoopTime");
            if (loop != null) { loop.boolValue = true; so.ApplyModifiedProperties(); }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Locomotion Asignado", "Se asignaron Standing/Walking/Running al Blend Tree de Locomotion del objeto seleccionado.", "OK");
    }
}