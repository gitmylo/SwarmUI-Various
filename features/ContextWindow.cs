using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Text2Image;

namespace GitMylo.SwarmVarious.features;

public class ContextWindow : ComfyNodeFeature
{
    // Handle compatibility in swarm so the same can be used, the only difference between the comfy nodes is input tweaks, also stage restriction to limit to one generation stage
    public static T2IRegisteredParam<string> StageRestriction;
    public static T2IRegisteredParam<string> CompatibilityMode;
    
    // Actual options
    public static T2IRegisteredParam<int> ContextLength;
    public static T2IRegisteredParam<int> ContextOverlap;
    public static T2IRegisteredParam<string> ContextSchedule;
    public static T2IRegisteredParam<int> ContextStride;
    public static T2IRegisteredParam<bool> ClosedLoop;
    public static T2IRegisteredParam<string> FuseMethod;
    public static T2IRegisteredParam<int> Dimension;
    public static T2IRegisteredParam<bool> FreeNoise;

    public static T2IParamGroup ContextWindowGroup;
    
    public override void Register()
    {
        ContextWindowGroup = new T2IParamGroup("Manual context window", true, false, Description: "Allows for manually setting the context window, allowing generating longer videos for example.", Parent: SwarmVariousExtension.SwarmVariousGroup);

        StageRestriction = T2IParamTypes.Register<string>(new ("Restrict to generation stage", "Only apply the context window to this model, disable for all.", "base+refiner", GetValues:Values("base+refiner", "img2vid"),
            OrderPriority:-2, Group:ContextWindowGroup, Toggleable:true));
        
        CompatibilityMode = T2IParamTypes.Register<string>(
            new ("Compatibility mode", "The compatibility mode for a model, currently only wan. Compatibility means the settings are corrected if invalid for the model.",
                "Wan", Toggleable: true, Group: ContextWindowGroup, GetValues: Values("Wan"), OrderPriority:-1)
            );
        
        // Actual options
        ContextLength = T2IParamTypes.Register<int>(new ("Context length", "The length of the context in memory, the amount of context being processed as one.", "81", Min:1, Max:99999, Group:ContextWindowGroup, OrderPriority:0));
        ContextOverlap = T2IParamTypes.Register<int>(new("Context overlap", "The overlap to use when shifting context, basically the number of shared frames between contexts.", "30", Min:0, Max:99999, Group:ContextWindowGroup, OrderPriority:1));
        ContextSchedule = T2IParamTypes.Register<string>(new ("Context schedule", "The type of movement the context makes over the full context.", "standard_static", GetValues: Values("standard_static", "standard_uniform", "looped_uniform", "batched"), Group:ContextWindowGroup, OrderPriority:2));
        ContextStride = T2IParamTypes.Register<int>(new ("Context stride", "(Not entirely sure) The added size of the gap between context jumps. Could also be a gap between context entries.", "1", Min:1, Max:99999, Group:ContextWindowGroup, OrderPriority:3));
        ClosedLoop = T2IParamTypes.Register<bool>(new ("Closed loop", "Makes the context loop, requires a looped schedule, otherwise it doesn't work.", "false", Group:ContextWindowGroup, OrderPriority:4));
        FuseMethod = T2IParamTypes.Register<string>(new ("Fuse method", "The method of fusing contexts together.", "pyramid", GetValues:Values("pyramid", "relative", "flat", "overlap-linear"), Group:ContextWindowGroup, OrderPriority:5));
        Dimension = T2IParamTypes.Register<int>(new("Dimension", "The dimension to apply the context on, for wan this should be 2, if wan compatibility mode is enabled, it's always 2.", "2", Min:0, Max:5, Group:ContextWindowGroup, OrderPriority:6));
        FreeNoise = T2IParamTypes.Register<bool>(new("Use freenoise", "Uses freenoise, this could give better results.", "false", Group:ContextWindowGroup, OrderPriority:7));
        
        WorkflowGeneratorSteps.AddModelGenStep(g =>
        {
            if (g.UserInput.TryGet(StageRestriction, out string restriction))
            {
                // Base, Refiner
                // image2video
                switch (restriction)
                {
                    case "base+refiner":
                        if (g.LoadingModelType != "Base" && g.LoadingModelType != "Refiner") return;
                        break;
                    case "img2vid":
                        if (g.LoadingModelType != "image2video") return;
                        break;
                }
            }

            int contextLength, contextOverlap, contextStride, dimension;
            string contextSchedule, fuseMethod;
            bool closedLoop, freeNoise;
            if (g.UserInput.TryGet(ContextLength, out int contextLength_) && g.UserInput.TryGet(ContextOverlap, out int contextOverlap_)
                && g.UserInput.TryGet(ContextSchedule, out string contextSchedule_) && g.UserInput.TryGet(ContextStride, out int contextStride_)
                && g.UserInput.TryGet(ClosedLoop, out bool closedLoop_) && g.UserInput.TryGet(FuseMethod, out string fuseMethod_)
                && g.UserInput.TryGet(Dimension, out int dimension_) && g.UserInput.TryGet(FreeNoise, out bool freeNoise_))
            {
                contextLength = contextLength_;
                contextOverlap = contextOverlap_;
                contextStride = contextStride_;
                dimension = dimension_;
                contextSchedule = contextSchedule_;
                fuseMethod = fuseMethod_;
                closedLoop = closedLoop_;
                freeNoise = freeNoise_;
            }
            else return; // Missing input, group is probably disabled

            if (g.UserInput.TryGet(CompatibilityMode, out string mode))
            {
                (contextLength, contextOverlap, dimension) = Compatibility(contextLength, contextOverlap, dimension, mode);
            }

            g.LoadingModel = [g.CreateNode("ContextWindowsManual", new JObject
            {
                ["model"] = g.LoadingModel,
                ["context_length"] = contextLength,
                ["context_overlap"] = contextOverlap,
                ["context_schedule"] = contextSchedule,
                ["context_stride"] = contextStride,
                ["closed_loop"] = closedLoop,
                ["fuse_method"] = fuseMethod,
                ["dim"] = dimension,
                ["freenoise"] = freeNoise,
            }), 0];
        }, -99);
    }

    /// <summary>
    /// Applies compatibility tweaks to inputs
    /// </summary>
    /// <returns>tuple with (length, overlap)</returns>
    private (int, int, int) Compatibility(int length, int overlap, int dim, string compmode)
    {
        switch (compmode)
        {
            case "Wan":
                return (Math.Max(((int)Math.Floor(((double)length - 1) / 4)*4) + 1, 1),
                    Math.Max(((int)Math.Floor(((double)overlap - 1) / 4)*4) + 1, 0),
                    2);
        }
        return (length, overlap, dim);
    }
}