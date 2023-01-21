#ifndef HLSL_COLORUTILITY_INCLUDED
#define HLSL_COLORUTILITY_INCLUDED


float3 RGBtoHSV(float3 rgb) {
    float3 hsv;
    float minVal, maxVal, delta;

    minVal = min(rgb.x, min(rgb.y, rgb.z));
    maxVal = max(rgb.x, max(rgb.y, rgb.z));

    hsv.z = maxVal;				// v
    delta = maxVal - minVal;

    if (maxVal != 0)
        hsv.y = delta / maxVal;		// s
    else {
        hsv.y = 0;
        hsv.x = -1;
        return hsv;
    }

    if (rgb.x == maxVal)
        hsv.x = (rgb.y - rgb.z) / delta;		// between yellow & magenta
    else if (rgb.y == maxVal)
        hsv.x = 2 + (rgb.z - rgb.x) / delta;	// between cyan & yellow
    else
        hsv.x = 4 + (rgb.x - rgb.y) / delta;	// between magenta & cyan

    hsv.x *= 60;				// degrees
    if (hsv.x < 0)
        hsv.x += 360;

    return hsv;
}

float RGBtoValue(float3 rgb) {
    return max(rgb.x, max(rgb.y, rgb.z));
}

float RGBtoSaturation(float3 rgb) {
    float maxVal = max(rgb.x, max(rgb.y, rgb.z));

    if (maxVal != 0) {
        float minVal = min(rgb.x, min(rgb.y, rgb.z));
        float delta = maxVal - minVal;
        return delta / maxVal;		// s
    } else
        return 0;
}

float3 HSVtoRGB(float3 HSV) {
    float3 RGB = HSV.z;

    float var_h = HSV.x * 6;
    float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
    float var_1 = HSV.z * (1.0 - HSV.y);
    float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
    float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
    if (var_i == 0) 
        RGB = float3(HSV.z, var_3, var_1);
    else if (var_i == 1) 
        RGB = float3(var_2, HSV.z, var_1);
    else if (var_i == 2) 
        RGB = float3(var_1, HSV.z, var_3);
    else if (var_i == 3) 
        RGB = float3(var_1, var_2, HSV.z);
    else if (var_i == 4) 
        RGB = float3(var_3, var_1, HSV.z);
    else                 
        RGB = float3(HSV.z, var_1, var_2);
    
    return (RGB);
}


float4 ColorSaturation(float4 color, float saturation) {
    if (saturation == 0)
        return color;
        
    // if (RGBtoSaturation(color.rgb) == 0)
    //     return color;
        
    float3 hsv = RGBtoHSV(color.rgb);

    // hsv.y += saturation;
    // float4 saturatedColor = saturate(float4( HSVtoRGB(hsv), color.a ));

    float4 saturatedColor = float4(HSVtoRGB(float3(hsv.r, saturation, hsv.b)), color.a);

    return saturate(saturatedColor);
}

float3 ColorSaturation(float3 color, float saturation) {
    if (saturation == 0)
        return color;
        
    // if (RGBtoSaturation(color) == 0)
    //     return color;

    float3 hsv = RGBtoHSV(color);

    // hsv.y += saturation;
    // float3 saturatedColor = saturate(HSVtoRGB(hsv));

    float3 saturatedColor = HSVtoRGB(float3(hsv.r, saturation, hsv.b));

    return saturate(saturatedColor);
}

float3 SaturateColor(float3 color, float saturation) {
    
    float3 intensity = dot(color, float3(0.299,0.587,0.114));
    return lerp(intensity, color, saturation);

}
    // float4 saturatedColor = mix(
    //     float3( dot(color, float3(0.299, 0.587, 0.114)) ),
    //     color,
    //     saturation
    // )
    // saturatedColor.a = color.a;
    // return saturatedColor;


#endif