# DeeMultipleShadow
### 1.Dee_PlanerShadow 平面阴影
使用三角形相似原理计算阴影在reciever上的xz坐标。
![](https://github.com/OgreDee/DeeMultipleShadow/blob/master/Movie/PlanarShadow01.gif)
>[Demo场景](https://github.com/OgreDee/DeeMultipleShadow/blob/master/DeeMultipleShadow/Assets/Dee_PlanerShadow/Dee_ShadowPlaner.unity)
>参考:
1. [使用顶点投射的方法制作实时阴影](https://zhuanlan.zhihu.com/p/31504088)
2. ShaderLab开发实战详解，10.1平行光对平面的投影
>缺点:
>不支持自阴影

## 2.体积阴影（待加）
## 3.基于ShadowTexture的阴影（待加）
## 4.基于ShadowMap的阴影
![](https://github.com/OgreDee/DeeMultipleShadow/blob/master/Movie/shadowmap01.gif)
>[Demo场景](https://github.com/OgreDee/DeeMultipleShadow/blob/master/DeeMultipleShadow/Assets/Dee_ShadowMap/Dee_LightCamera/Dee_ShadowMap01.unity)
1. 从光源的视角渲染整个场景，获得Shadow Map
2. 实际相机渲染物体，将物体从世界坐标转换到光源视角下，与深度纹理对比数据获得阴影信息
3. 根据阴影信息渲染场景以及阴影
>参考:
[Unity基础6 Shadow Map 阴影实现](https://www.cnblogs.com/zsb517/p/6817373.html)
## 5.基于Projector的阴影（待加）
## 6.PCF抗锯齿（待加）
## 7.VSM抗锯齿（待加）
## 8.PCSS
## 9.CSM级联阴影（待加）
## 10.屏幕空间阴影
## 11.角色高清阴影（待加）
