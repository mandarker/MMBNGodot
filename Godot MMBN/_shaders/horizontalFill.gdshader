shader_type canvas_item;

uniform float _fill;
uniform bool _pixelPerfect;
uniform float _pixels;

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 modifiedUV = UV;
	
	COLOR = texture(TEXTURE, modifiedUV);
	
	if (_pixelPerfect)
	{
		modifiedUV.x = modifiedUV.x * _pixels;
		modifiedUV.x = floor(modifiedUV.x) + 1.0f;
		modifiedUV.x /= _pixels;
	}
	
	if (_fill < modifiedUV.x)
	{
		COLOR.a = 0.0f;
	}
}
