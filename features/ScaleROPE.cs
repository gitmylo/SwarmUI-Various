using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Text2Image;

namespace GitMylo.SwarmVarious.features;

/// <summary>
/// Applies rope scaling, currently only supported on wan and lumina image, but should be usable with other transformer-based models in the future.
/// </summary>
public class ScaleROPE : ComfyNodeFeature
{
    public static T2IRegisteredParam<float> ROPEScaleX;
    public static T2IRegisteredParam<float> ROPEShiftX;
    
    public static T2IRegisteredParam<float> ROPEScaleY;
    public static T2IRegisteredParam<float> ROPEShiftY;
    
    public static T2IRegisteredParam<float> ROPEScaleT;
    public static T2IRegisteredParam<float> ROPEShiftT;
    

    public static T2IParamGroup ROPEGroup;
    
    public override void Register()
    {
        ROPEGroup = new T2IParamGroup("ROPE scaling", true, false, Description: "ROPE scaling for transformer based models", IsAdvanced: true, Parent:SwarmVariousExtension.SwarmVariousGroup);
        
        ROPEScaleX = T2IParamTypes.Register<float>(new("Scale X", "The horizontal scale.",
            "1.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: 0, Max: 100, Step: 0.1, OrderPriority:0
        ));
        ROPEShiftX = T2IParamTypes.Register<float>(new("Shift X", "The horizontal offset.",
            "0.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: -256, Max: 256, Step: 0.1, OrderPriority:1
        ));
        
        ROPEScaleY = T2IParamTypes.Register<float>(new("Scale Y", "The vertical scale.",
            "1.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: 0, Max: 100, Step: 0.1, OrderPriority:2
        ));
        ROPEShiftY = T2IParamTypes.Register<float>(new("Shift Y", "The vertical offset.",
            "0.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: -256, Max: 256, Step: 0.1, OrderPriority:3
        ));
        
        ROPEScaleT = T2IParamTypes.Register<float>(new("Scale T", "The temporal scale.",
            "1.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: 0, Max: 100, Step: 0.1, OrderPriority:4
        ));
        ROPEShiftT = T2IParamTypes.Register<float>(new("Shift T", "The temporal offset.",
            "0.0", Toggleable: false, Group: ROPEGroup, FeatureFlag: "comfyui",
            Min: -256, Max: 256, Step: 0.1, OrderPriority:5
        ));
        
        WorkflowGeneratorSteps.AddModelGenStep(g =>
        {
            if (
                g.UserInput.TryGet(ROPEScaleX, out var scaleX) && g.UserInput.TryGet(ROPEShiftX, out var shiftX) &&
                g.UserInput.TryGet(ROPEScaleY, out var scaleY) && g.UserInput.TryGet(ROPEShiftY, out var shiftY) &&
                g.UserInput.TryGet(ROPEScaleT, out var scaleT) && g.UserInput.TryGet(ROPEShiftT, out var shiftT))
            {
                string scaleNode = g.CreateNode("ScaleROPE", new JObject
                {
                    ["model"] = g.LoadingModel,
                    ["scale_x"] = scaleX,
                    ["shift_x"] = shiftX,
                    ["scale_y"] = scaleY,
                    ["shift_y"] = shiftY,
                    ["scale_t"] = scaleT,
                    ["shift_t"] = shiftT,
                });
                g.FinalModel = [scaleNode, 0];
                g.LoadingModel = [scaleNode, 0];
            }
        }, -13);
    }
}