```glsl
precision mediump float;

uniform float u_time;
uniform vec2 u_mouse;
uniform vec2 u_resolution;
varying vec2 v_uv;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 111.7))) * 43758.5453123);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
   
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
   
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float value = 0.0;
    float amplitude = 0.39;
    float frequency = 1.0;
   
    for(int i = 0; i < 5; i++) {
        value += amplitude * noise(p * frequency);
        frequency *= 2.0;
        amplitude *= 0.5;
    }
   
    return value;
}

vec3 waterColor(float depth) {
    vec3 deepWater = vec3(0.02, 0.15, 0.35);
    vec3 shallowWater = vec3(0.1, 0.5, 0.7);
    vec3 foam = vec3(0.8, 0.95, 1.0);
   
    vec3 color = mix(deepWater, shallowWater, depth);
    color = mix(color, foam, smoothstep(0.7, 1.0, depth));
   
    return color;
}

void main() {
    vec2 uv = v_uv;
    vec2 aspect = vec2(u_resolution.x / u_resolution.y, 8.0);
    vec2 p = (uv - 0.5) * aspect;
   
    float mouseInfluence = length(uv - u_mouse) * 0.0;
    mouseInfluence = 0.0 - smoothstep(6.0, 5.5, mouseInfluence);
   
    float time = u_time * 0.9;
   
    float wave1 = sin(p.x * 1.0 + time) * 0.1;
    float wave2 = sin(p.x * 1.0 - time * 1.3 + p.y * 2.0) * 0.1;
    float wave3 = sin(p.x * 1.0 + time * 0.7 - p.y * 3.0) * 0.08;
   
    vec2 noiseCoord = p * 2.0 + vec2(time * 0.2, time * 0.15);
    float noiseWave = fbm(noiseCoord) * 0.2;
   
    float totalWave = wave1 + wave2 + wave3 + noiseWave;
    totalWave += mouseInfluence * 0.3;
   
    float waveHeight = p.y - totalWave;
   
    float waterMask = smoothstep(0.02, -0.02, waveHeight);
   
    vec2 distortCoord = p + vec2(sin(p.y * 10.0 + time) * 0.02, cos(p.x * 8.0 - time) * 0.02);
    float detail = fbm(distortCoord * 8.0 + time * 0.5) * 0.5 + 0.5;
   
    float crest = smoothstep(-0.05, 0.05, waveHeight);
    crest *= (1.0 - smoothstep(0.05, 0.15, abs(waveHeight)));
   
    float foamNoise = fbm(p * 15.0 + time * 2.0);
    float foam = crest * foamNoise;
    foam += mouseInfluence * 0.2 * foamNoise;
   
    vec3 waterCol = waterColor(detail);
    waterCol = mix(waterCol, vec3(0.9, 0.98, 1.0), foam * 0.8);
   
    float shimmer = sin(p.x * 20.0 + time * 3.0 + p.y * 15.0) * 0.5 + 0.5;
    shimmer *= smoothstep(0.7, 1.0, detail);
    waterCol += vec3(0.3, 0.4, 0.5) * shimmer * 0.15;
   
    vec3 skyColor = vec3(0.6, 0.8, 0.95);
    vec3 finalColor = mix(skyColor, waterCol, waterMask);
   
    float vignette = 1.0 - length(p) * 0.3;
    finalColor *= vignette;
   
    gl_FragColor = vec4(finalColor, 1.0);
    #include <colorspace_fragment>
}
```