bl_info = {
	"name" : "XAT",
	"author" : "Asgard",
	"description" : "Export custom animations for FFXIV",
	"version": (1, 0, 0),
	"blender" : (3, 0, 0),
	"location" : "3D View > Tools (Right Side) > XAT",
	"warning" : "",
	"category" : "Animation",
	"wiki_url": 'https://github.com/AsgardXIV/XAT',
    "tracker_url": 'hhttps://github.com/AsgardXIV/XAT',
}

from .camera import camera

def register():
	camera.register()

def unregister():
    camera.unregister()