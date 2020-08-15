#version 100

precision highp float;
precision mediump int;

uniform sampler2D texture;

varying vec2 texCoord;
varying vec4 color;

uniform vec2 rt_dims;

vec4 blur9(sampler2D image, vec2 uv, vec2 resolution, vec2 direction) {
    vec4 color = vec4(0.0);
    vec2 off1 = vec2(1.3846153846) * direction;
    vec2 off2 = vec2(3.2307692308) * direction;
    color += texture2D(image, uv) * 0.2270270270;
    color += texture2D(image, uv + (off1 / resolution)) * 0.3162162162;
    color += texture2D(image, uv - (off1 / resolution)) * 0.3162162162;
    color += texture2D(image, uv + (off2 / resolution)) * 0.0702702703;
    color += texture2D(image, uv - (off2 / resolution)) * 0.0702702703;
    return color;
}

void main()
{
    vec4 c = blur9(texture, texCoord, rt_dims, vec2(0.75, 0.75));
    c += blur9(texture, texCoord, rt_dims, vec2(-0.75, -0.75)) / 2.0;
    gl_FragColor = c;
}