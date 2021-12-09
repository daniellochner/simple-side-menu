// Simple Side-Menu - https://assetstore.unity.com/packages/tools/gui/simple-side-menu-143623
// Copyright (c) Daniel Lochner

using UnityEditor;
using UnityEngine;

namespace DanielLochner.Assets.SimpleSideMenu
{
    [CustomEditor(typeof(SimpleSideMenu))]
    public class SimpleSideMenuEditor : SSMCopyrightEditor
    {
        #region Fields
        private bool showBasicSettings = true, showDragSettings = true, showOverlaySettings = true, showEvents = false;
        private SerializedProperty placement, defaultState, transitionSpeed, thresholdDragSpeed, thresholdDragDistance, thresholdDraggedFraction, handle, isHandleDraggable, handleToggleStateOnPressed, isMenuDraggable, useOverlay, overlayColour, useBlur, blurMaterial, blurRadius, overlaySwipe, overlayRetractOnPressed, onStateChanged, onStateSelected, onStateChanging, onStateSelecting;
        private SimpleSideMenu sideMenu;
        private State editorState;
        #endregion

        #region Methods
        private void OnEnable()
        {
            sideMenu = target as SimpleSideMenu;

            #region Serialized Properties
            placement = serializedObject.FindProperty("placement");
            defaultState = serializedObject.FindProperty("defaultState");
            transitionSpeed = serializedObject.FindProperty("transitionSpeed");
            thresholdDragSpeed = serializedObject.FindProperty("thresholdDragSpeed");
            thresholdDraggedFraction = serializedObject.FindProperty("thresholdDraggedFraction");
            handle = serializedObject.FindProperty("handle");
            isHandleDraggable = serializedObject.FindProperty("isHandleDraggable");
            handleToggleStateOnPressed = serializedObject.FindProperty("handleToggleStateOnPressed");
            isMenuDraggable = serializedObject.FindProperty("isMenuDraggable");
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
            PrefabUtility.RecordPrefabInstancePropertyModifications(sideMenu);
        }

        private void ShowCurrentStateSettings()
        {
            editorState = (Application.isPlaying) ? sideMenu.TargetState : sideMenu.DefaultState;
            #region Close
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(editorState == State.Closed))
            {
                if (GUILayout.Button("Close"))
                {
                    sideMenu.Close();
                    if (!Application.isPlaying)
                    {
                        sideMenu.DefaultState = State.Closed;
                    }
                }
            }
            #endregion
            #region Toggle State
            if (GUILayout.Button("Toggle State"))
            {
                sideMenu.ToggleState();
                if (!Application.isPlaying)
                {
                    sideMenu.DefaultState = (sideMenu.DefaultState == State.Closed) ? State.Open : State.Closed;
                }
            }
            #endregion
            #region Open
            using (new EditorGUI.DisabledScope(editorState == State.Open))
            {
                if (GUILayout.Button("Open"))
                {
                    sideMenu.Open();
                    if (!Application.isPlaying)
                    {
                        sideMenu.DefaultState = State.Open;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            #endregion
        }
        private void ShowBasicSettings()
        {
            EditorLayoutUtility.Header(ref showBasicSettings, new GUIContent("Basic Settings"));
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
            EditorLayoutUtility.Header(ref showDragSettings, new GUIContent("Drag Settings"));
            if (showDragSettings)
            {
                EditorGUILayout.PropertyField(thresholdDragSpeed, new GUIContent("Threshold Drag Speed", "The minimum speed required when dragging that will allow a transition to the next state to occur."));
                EditorGUILayout.Slider(thresholdDraggedFraction, 0f, 1f, new GUIContent("Threshold Dragged Fraction", "The fraction of the fully opened menu that must be dragged before a transition will occur to the next state if the current drag speed does not exceed the threshold drag speed set."));
                EditorGUILayout.ObjectField(handle, typeof(GameObject), new GUIContent("Handle", "(Optional) GameObject used to open and close the side menu by dragging or pressing (when a \"Button\" component has been added)."));
                if (sideMenu.Handle != null)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(isHandleDraggable, new GUIContent("Is Draggable", "Should the handle be able to be used to drag the Side-Menu?"));
                    EditorGUILayout.PropertyField(handleToggleStateOnPressed, new GUIContent("Toggle State on Pressed", "Should the Side-Menu toggle its state (open/close) when the handle is pressed?"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(isMenuDraggable, new GUIContent("Is Menu Draggable", "Should the Side-Menu (itself) be able to be used to drag the Side-Menu?"));
            }
            EditorGUILayout.Space();
        }
        private void ShowOverlaySettings()
        {
            EditorLayoutUtility.Header(ref showOverlaySettings, new GUIContent("Overlay Settings"));
            if (showOverlaySettings)
            {
                EditorGUILayout.PropertyField(useOverlay, new GUIContent("Use Overlay", "Should an overlay be used when the Side-Menu is opened/closed?"));
                if (sideMenu.UseOverlay)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(overlayColour, new GUIContent("Colour", "The colour of the overlay when fully opened."));
                    EditorGUILayout.PropertyField(useBlur, new GUIContent("Use Blur", "Should a blur effect be applied to the overlay?"));
                    if (sideMenu.UseBlur)
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
            EditorLayoutUtility.Header(ref showEvents, new GUIContent("Events"));
            if (showEvents)
            {
                EditorGUILayout.PropertyField(onStateSelecting);
                EditorGUILayout.PropertyField(onStateSelected);
                EditorGUILayout.PropertyField(onStateChanging);
                EditorGUILayout.PropertyField(onStateChanged);
            }
        }
        #endregion
    }
}