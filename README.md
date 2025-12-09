# SwarmUI-Various

A [SwarmUI](https://github.com/mcmonkeyprojects/SwarmUI) extension which adds various new settings.

## What is this extension?
This extension is a mixed extension of various features that swarm does not currently support, if swarm ends up adding a feature that's in this extension, I'll remove it from the extension.

## Current features
* ROPE scaling comfy node
  * A recently added comfy node for rope scaling, which allows for rescaling the x, y and time dimensions on a video, which means you can generate a video at any fps with wan for example (although wan's vae seems to cause jittering currently)
  * ComfyUI currently only supports ROPE for wan and lumina image. But other transformer based models can be supported in the future.
* Context windows comfy node
  * A \[BETA] comfyui node which allows for longer context windows when generating which allows for longer video generations.
  * With a compatibility option for WAN which makes it work like the Wan variant of the node, note that it still uses the same actual node.

More coming soon!
