if(jsobject != undefined) {
	$(document).keydown(function(e) { jsobject.OnKeyDown(e.which); });
	$(document).keyup  (function(e) { jsobject.OnKeyUp  (e.which); });
}