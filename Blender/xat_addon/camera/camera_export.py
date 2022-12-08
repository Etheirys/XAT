from bpy.types import Context, Object
from bpy_extras.io_utils import axis_conversion
from math import radians
from mathutils import Matrix, Quaternion, Vector, Euler
from ..utils import io_helper

class CameraFrame(object):
    def __init__(self):
        self.translation = Vector((0, 0, 0))
        self.rotation = Quaternion((0, 0, 0, 0))
        self.fov = 0.0

    def write(self, file):
        v = self.translation
        v = (v.x, v.y, v.z)
        q = self.rotation
        q = (q.x, q.y, q.z, q.w)

        io_helper.write_vector3(file, v)
        io_helper.write_vector4(file, q)
        io_helper.write_float(file, self.fov)

def export(context: Context, camera: Object, out_filepath):
    context.view_layer.objects.active = camera
    context.active_object.select_set(state=True)

    context.scene.frame_step

    total_frames = (context.scene.frame_end - context.scene.frame_start) + 1

    current_frame = 0
    key_frames = []

    axis_convert = axis_conversion(from_forward='-Y', 
        from_up='Z',
        to_forward='Z',
        to_up='Y').to_4x4()

    for current_frame in range(1, total_frames + 1):
        context.scene.frame_set(current_frame)
        cur = camera.matrix_world
        cur = axis_convert @ cur

        f = CameraFrame()
        f.translation = cur.to_translation()
        f.rotation = cur.to_quaternion()
        f.fov = camera.data.angle

        key_frames.append(f)

    with open(out_filepath, 'wb') as file:
        io_helper.write_string(file, "XCP1")
        io_helper.write_int(file, total_frames)
            
        for frame in key_frames:
            frame.write(file)