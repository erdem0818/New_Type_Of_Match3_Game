using UI.Li;
using UI.Li.Common;
using UI.Li.Editor;
using UI.Li.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

public class TestWindow: ComposableWindow
{
    [MenuItem("Lithium/Examples/TestWindow")]
    public static void ShowWindow() => GetWindow<TestWindow>();

    protected override string WindowName => "Window Test";
    
    protected override IComponent Layout() => CU.Flex(
        direction: FlexDirection.Row,
        content: new [] { ToggleButton(), ToggleButton() , SampleButton()}
    );

    private static IComponent SampleButton()
    {
        return CU.Button(() => Debug.Log("Click"), "Sample Button");
    }
    
    /*private static IComponent ToggleButton()
    {
        return CU.Toggle(
            OnValueChanged,
            false,
            new IManipulator[] { new Blurrable((() =>
            {
                Debug.Log("BLUR");
            }))});
    }

    private static void OnValueChanged(bool b)
    {
        Debug.Log($"bool: {b}");
    }*/

    private static IComponent ToggleButton()
    {
        return new UI.Li.Component(state =>
        {
            var isOn = state.Remember(false);
            
            return CU.Button(
                onClick: () => isOn.Value = !isOn,
                content: isOn ? "On" : "Off");
/*
            return CU.Button(
                data: new(flexGrow: 1),
                content: isOn ? "On" : "Off",
                onClick: () => isOn.Value = !isOn
            );
*/
        });
    }
}