#version 330

out vec4 LightMap;       

uniform vec2 ScreenSize;

uniform float LightRadius;
uniform vec3 LightColor;
uniform float LightIntensity;
uniform vec3 LightCenter;

uniform sampler2D ColorBuffer;
uniform sampler2D PositionBuffer;
uniform sampler2D NormalBuffer;

void main()
{
    vec2 texCoord = gl_FragCoord.xy / ScreenSize;

    vec3 pixelPos = texture2D(PositionBuffer,texCoord).xyz;
    vec3 pixelNormal = normalize(texture2D(NormalBuffer, texCoord).xyz);
    vec3 diffuseColor = texture2D(ColorBuffer, texCoord).xyz;

    vec3 toLight = LightCenter - pixelPos;

    float attenuation = clamp(1.0 - length(toLight)/LightRadius,0.0,1.0); 

    toLight = normalize(toLight);

    float nDotL = max(dot(pixelNormal, toLight),0.0);

    vec3 diffuseLight = diffuseColor * nDotL;

    LightMap = LightIntensity * attenuation * vec4(diffuseLight,1.0);
}

