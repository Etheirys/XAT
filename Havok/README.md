# XATHavokInterop

This tool interacts with Havok files. 
It is not intended to be used directly by humans but should be wrapped by higher level libraries/tools.

If you need to interpret Havok files you should use the XAT.Common library or write your own implementation.

This tool is only intended to do the bare minimum and leave higher level logic to other libraries. 
For example, it assumes when converting animations that the container has already been combined even though this is never the case for animations from paps. 
The goal is to expose the minimum level of functionality that can then be composed by other tools for more convenience.

Input is defined by arguments on the command line.
Human readable output is piped to stdout. Machine readable output is piped to stderr.
The tool never overrwrites any files used as source unless they are specified as the output.

## API
```
Get stats about the container - getStats(string container) -> (int skeletonCount / int animCount / int bindingCount)
Create a blank animation container - createContainer(string outputPath) -> null
Add animation and associated binding to container - addAnimation(string targetContainer, string sourceContainer, int sourceAnimIdx, string outputContainer) -> (int newAnimIdx / int newBindingIdx)
Replace an animation and associated binding in container - replaceAnimation(string targetContainer, int targetAnimIdx, string sourceContainer, int sourceAnimIdx, string outputContainer) -> (int newAnimIdx / int newBindingIdx)
Remove animation and associated binding from container - removeAnimation(string targetContainer, int animIdx, string outputContainer) -> (int newAnimAcount / int newBindingCount)
Remove skeleton from container - removeSkeleton(string targetContainer, int skeletonIdx, string outputContainer) -> (int newSkeleCount)
Add skeleton to container - addSkeleton(string targetContainer, string sourceContainer, int skeletonIdx, string outputContainer) -> (int newSkeleCount)
Get list of bones in index order - listBones(string container, int skeletonIdx) -> (list<string />)
Convert havok container to binary tagfile - toTagFile(string sourceContainer, string outputContainer) -> null
Convert havok container to xml tagfile - toXMLFile(string sourceContainer, string outputContainer) -> null
Convert havok container to packfile (HavokMax compatible) - toPackFile(string sourceContainer, string outputContainer) -> null
Convert havok skeleton to fbx - toFbxSkeleton(string container, int skeletonIdx, string outputPath) -> (int bonesConverted)
Convert havok animation to fbx - toFbxAnimation(string container, int skeletonIdx, int animIdx, string outputPath) -> (int boneConverted, int framesConverted)
Get list of FBX anim stacks in order - listFbxStacks(string fbxPath) -> (list<string />)
Convert FBX skeleton to havok - fromFbxSkeleton(string fbxPath, string boneOrder, string outputPath) -> (int bonesConverted)
Convert FBX to havok animation - fromFbxAnimation(string fbxPath, int animStackIdx, string sourceSkeleton, int skeletonIdx, string excludeBones, string outputPath) -> (int framesConverted, int bonesBound)
Get list of FBX bones in index order - listFbxBones(string container) -> (list<string />)
Quanitized compression - compress(const quantized, string sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance, string outputContainerPath) -> null
Predictive compression - compress(const predictive, string sourceContainerPath, int sourceAnimIdx, int sourceSkeletonIdx, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance, string outputContainerPath) -> null
```

## Building
* Visual C++ Platform Toolset v110 (Visual Studio 2012)
* Havok 2014 SDK for VS2012 (set as HAVOK_SDK_ROOT environment variable)
* [FBX SDK 2014.2.1](https://www.autodesk.com/developer-network/platform-technologies/fbx-sdk-2014-2-1) for VS2012 (set as FBX_SDK_ROOT environment variable)