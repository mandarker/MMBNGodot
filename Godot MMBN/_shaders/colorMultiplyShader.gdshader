shader_type canvas_item;

uniform vec4 _col;
uniform vec4 _colTint;

uniform bool _white;
uniform bool _clear;
uniform bool _flash;
uniform float _tintStrength;

void fragment() {
	COLOR = texture(TEXTURE, UV);
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
	
	if (_clear)
	{
		COLOR = vec4(COLOR.r, COLOR.g, COLOR.b, 0);
	}
}
