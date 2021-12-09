// Simple Side-Menu - https://assetstore.unity.com/packages/tools/gui/simple-side-menu-143623
// Copyright (c) Daniel Lochner

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DanielLochner.Assets.SimpleSideMenu
{
    [CustomEditor(typeof(SimpleSideMenu))]
    public class SimpleSideMenuEditor : SSMCopyrightEditor
    {
        #region Fields
        private bool showBasicSettings = true, showDragSettings = true, showOverlaySettings = true, showEvents = false;
        private SerializedProperty placement, defaultState, transitionSpeed, thresholdDragSpeed, thresholdDragDistance, thresholdDraggedFraction, handle, handleDraggable, handleToggleStateOnPressed, menuDraggable, useOverlay, overlayColour, useBlur, blurMaterial, blurRadius, overlaySwipe, overlayRetractOnPressed, onStateChanged, onStateSelected, onStateChanging, onStateSelecting;
        private State editorState;
        private SimpleSideMenu simpleSideMenu;
        #endregion

        #region Methods
        private void OnEnable()
        {
            simpleSideMenu = target as SimpleSideMenu;

            #region Serialized Properties
            placement = serializedObject.FindProperty("placement");
            defaultState = serializedObject.FindProperty("defaultState");
            transitionSpeed = serializedObject.FindProperty("transitionSpeed");
            thresholdDragSpeed = serializedObject.FindProperty("thresholdDragSpeed");
            thresholdDraggedFraction = serializedObject.FindProperty("thresholdDraggedFraction");
            handle = serializedObject.FindProperty("handle");
            handleDraggable = serializedObject.FindProperty("handleDraggable");
            handleToggleStateOnPressed = serializedObject.FindProperty("handleToggleStateOnPressed");
            menuDraggable = serializedObject.FindProperty("menuDraggable");
            useOverlay = serializedObject.FindProperty("useOverlay");
            overlayColour = serializedObject.FindProperty("overlayColour");
            useBlur = serializedObject.FindProperty("useBlur");
            blurMaterial = serializedObject.FindProperty("blurMaterial");
            blurRadius = serializedObject.FindProperty("blurRadius");
            overlayRetractOnPressed = serializedObject.FindProperty("overlayCloseOnPressed");
            onStateSelected = serializedObject.FindProperty("onStateSelected");
            onStateSelecting = serializedObject.FindProperty("onStateSelecting");
            onStateChanging = serializedObject.FindProperty("onStateChanging");
            onStateChanged = serializedObject.FindProperty("onStateChanged");
            #endregion
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShowCopyrightNotice();
            ShowCurrentStateSettings();
            ShowBasicSettings();
            ShowDragSettings();
            ShowOverlaySettings();
            ShowEvents();

            serializedObject.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(simpleSideMenu);
        }

        private void ShowCurrentStateSettings()
        {
            editorState = (Application.isPlaying) ? simpleSideMenu.TargetState : simpleSideMenu.DefaultState;
            #region Close
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(editorState == State.Closed))
            {
                if (GUILayout.Button("Close"))
                {
                    simpleSideMenu.Close();
                    if (!Application.isPlaying) simpleSideMenu.DefaultState = State.Closed;
                }
            }
            #endregion
            #region Toggle State
            if (GUILayout.Button("Toggle State"))
            {
                simpleSideMenu.ToggleState();
                if (!Application.isPlaying)
                {
                    switch (simpleSideMenu.DefaultState)
                    {
                        case State.Closed:
                            simpleSideMenu.DefaultState = State.Open;
                            break;
                        case State.Open:
                            simpleSideMenu.DefaultState = State.Closed;
                            break;
                    }
                }
            }
            #endregion
            #region Open
            using (new EditorGUI.DisabledScope(editorState == State.Open))
            {
                if (GUILayout.Button("Open"))
                {
                    simpleSideMenu.Open();
                    if (!Application.isPlaying) simpleSideMenu.DefaultState = State.Open;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            #endregion
        }

        private void ShowBasicSettings()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "Basic Settings", true);
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (showBasicSettings)
            {
                EditorGUILayout.PropertyField(placement, new GUIContent("Placement", "The position at which the menu will be placed, which determines how the menu will be opened and closed."));
                EditorGUILayout.PropertyField(defaultState, new GUIContent("Default State", "Determines whether the menu will be open or closed by default."));
                EditorGUILayout.PropertyField(transitionSpeed, new GUIContent("Transition Speed", "The speed at which the menu will snap into position when transitioning to the next state."));
            }

            EditorGUILayout.Space();
        }
        private void ShowDragSettings()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            showDragSettings = EditorGUILayout.Foldout(showDragSettings, "Drag Settings", true);
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (showDragSettings)
            {
                EditorGUILayout.PropertyField(thresholdDragSpeed, new GUIContent("Threshold Drag Speed", "The minimum speed required when dragging that will allow a transition to the next state to occur."));
                EditorGUILayout.Slider(thresholdDraggedFraction, 0f, 1f, new GUIContent("Threshold Dragged Fraction", "The fraction of the fully opened menu that must be dragged before a transition will occur to the next state if the current drag speed does not exceed the threshold drag speed set."));
                EditorGUILayout.ObjectField(handle, typeof(GameObject), new GUIContent("Handle", "(Optional) GameObject used to open and close the side menu by dragging or pressing (when a \"Button\" component has been added)."));
                if (simpleSideMenu.Handle != null)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(handleDraggable, new GUIContent("Draggable", "Should the handle be able to be used to drag the Side-Menu?"));
                    EditorGUILayout.PropertyField(handleToggleStateOnPressed, new GUIContent("Toggle State on Pressed", "Should the Side-Menu toggle its state (open/close) when the handle is pressed?"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(menuDraggable, new GUIContent("Menu Draggable", "Should the Side-Menu (itself) be able to be used to drag the Side-Menu?"));
            }

            EditorGUILayout.Space();
        }
        private void ShowOverlaySettings()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            showOverlaySettings = EditorGUILayout.Foldout(showOverlaySettings, "Overlay Settings", true);
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (showOverlaySettings)
            {
                EditorGUILayout.PropertyField(useOverlay, new GUIContent("Use Overlay", "Should an overlay be used when the Side-Menu is opened/closed?"));
                if (simpleSideMenu.UseOverlay)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(overlayColour, new GUIContent("Colour", "The colour of the overlay when fully opened."));
                    EditorGUILayout.PropertyField(useBlur, new GUIContent("Blur", "Should a blur effect be applied to the overlay?"));
                    if (simpleSideMenu.UseBlur)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(blurMaterial, new GUIContent("Material", "The material applied to the background blur. For the default render pipeline, please use the material provided."));
                        EditorGUILayout.IntSlider(blurRadius, 0, 20, new GUIContent("Radius", "Set the radius of the blur (Warning: The larger the radius, the poorer the performance)."));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(overlayRetractOnPressed, new GUIContent("Close on Pressed", "Should the Side-Menu be closed when the overlay is pressed?"));
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();
        }

        private void ShowEvents()
        {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            showEvents = EditorGUILayout.Foldout(showEvents, "Event Handlers", true);
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (showEvents)
            {
                EditorGUILayout.PropertyField(onStateSelecting, new GUIContent("On State Selecting"));
                EditorGUILayout.PropertyField(onStateSelected, new GUIContent("On State Selected"));
                EditorGUILayout.PropertyField(onStateChanging, new GUIContent("On State Changing"));
                EditorGUILayout.PropertyField(onStateChanged, new GUIContent("On State Changed"));
            }
        }
        #endregion
    }
}