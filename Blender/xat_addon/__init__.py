bl_info = {
	"name" : "XAT",
	"author" : "Asgard",
	"description" : "Animations for FFXIV",
	"version": (2022,11,26,112),
	"blender" : (3, 0, 0),
	"location" : "3D View > Tools (Right Side) > XAT",
	"warning" : "",
	"category" : "Animation",
	"wiki_url": 'https://github.com/AsgardXIV/XAT',
    "tracker_url": 'https://github.com/AsgardXIV/XAT',
}

from . import xat

def register():
	xat.register()

def unregister():
    xat.unregister()
