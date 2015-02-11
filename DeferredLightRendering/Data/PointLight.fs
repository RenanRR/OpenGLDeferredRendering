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

    vec3 toLight = pixelPos - LightCenter;
    float pixelDist = length(toLight);
    toLight = normalize(toLight);

    pixelDist = clamp(1.0 - (pixelDist / LightRadius),0.0,1.0);

    vec3 pixelNormal = texture2D(NormalBuffer, texCoord).xyz;

    float nrm = dot(pixelNormal, -toLight);

    LightMap = texture2D(ColorBuffer, texCoord) * vec4(LightColor, 1) * LightIntensity * nrm * pixelDist;
}

