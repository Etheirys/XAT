import bpy
from bpy.types import Panel, Operator
from bpy.props import StringProperty
from bpy_extras.io_utils import ExportHelper

from .camera_export import export as export_camera

class XAT_PT_camera(Panel):
    bl_idname = "XAT_PT_camera"
    bl_label = "Camera"
    bl_category = "XAT"
    bl_space_type = "VIEW_3D"
    bl_region_type = "UI"

    def draw(self, context):
        obj = context.active_object
        layout = self.layout

        layout.operator(XAT_OT_camera_export.bl_idname, text="Export", icon="CAMERA_DATA")

class XAT_OT_camera_export(Operator, ExportHelper):
    bl_idname = "xat.camera_export"
    bl_label = "Export Camera"

    filename_ext = '.xcp'
    filter_glob: StringProperty(default='*.xcp', options={'HIDDEN'})

    @classmethod
    def poll(cls, context):
        obj = context.active_object
        return obj and (obj.type == 'CAMERA')

    def execute(self, context):
        obj = context.active_object
        export_camera(bpy.context, obj, self.filepath)

        return {'FINISHED'}     

classes = (XAT_PT_camera, XAT_OT_camera_export)

def register():
    from bpy.utils import register_class

    for cls in classes:
        register_class(cls)

def unregister():
    from bpy.utils import unregister_class


    for cls in classes:
        unregister_class(cls)