shader_type canvas_item;

uniform vec4 _col;
uniform vec4 _colTint;

uniform bool _white;
uniform float _transparency = 1;
uniform bool _flash;
uniform float _tintStrength;
uniform float _pixelationStrength;

void fragment() {
	// i love magic numbers
	vec2 modifiedUV = UV * vec2(1.0f/((_pixelationStrength + 0.001f)/100.0f));
	modifiedUV = vec2(float(int(modifiedUV.x)), float(int(modifiedUV.y)));
	modifiedUV = modifiedUV * vec2((_pixelationStrength + 0.001f)/100.0f);
	
	COLOR = texture(TEXTURE, modifiedUV);
	COLOR = COLOR * _col;
	
	COLOR = vec4(mix(COLOR.rgb, _colTint.rgb, _tintStrength), COLOR.a);
	
	if (_white)
	{
		COLOR = vec4(1, 1, 1, COLOR.a);
	}
	
	if (_flash)
	{
		float lerp = (sin(TIME * float(25)) + float(1)) / float(2);
		COLOR = vec4(COLOR.r, COLOR.g, COLOR.b, COLOR.a * round(lerp));	
	}
	
	COLOR = vec4(COLOR.r, COLOR.g, COLOR.b, COLOR.a * _transparency);
}
