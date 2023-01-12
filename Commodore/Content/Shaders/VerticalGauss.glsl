﻿#version 130

uniform sampler2D texture;

in vec2 texCoord;
in vec2 color;

uniform vec2 rt_dims;
uniform float vx_offset;

float offset[3] = float[](0.0, 1.3846153846, 3.2307692308);
float weight[3] = float[](0.2270270270, 0.3162162162, 0.0702702703);

out vec4 fragColor;

void main()
{
    vec3 tc = vec3(1.0, 0.0, 0.0);
    if (texCoord.x<(vx_offset-0.01))
    {
        vec2 uv = texCoord.xy;
        tc = texture2D(texture, uv).rgb * weight[0];

        for (int i=1; i<3; i++)
        {
            tc += texture2D(
                texture,
                uv + vec2(offset[i]) / rt_dims.x, 0.0
            ).rgb * weight[i];
            
            tc += texture2D(
                texture, 
                uv - vec2(offset[i]) / rt_dims.x, 0.0
            ).rgb * weight[i];
        }
    }
    else if (texCoord.x>=(vx_offset+0.01))
    {
        tc = texture2D(texture, texCoord.xy).rgb;
    }

    fragColor = vec4(tc, 1.0);
}