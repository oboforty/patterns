import os
import json
import lupa

_SCRIPT_LOC = "src_lua/"
_DAL_LOC = "app_flask/tmp/"


def filter_attribute_access(obj, attr_name, is_setting):
    if isinstance(attr_name, str):
        if not attr_name.startswith('_'):
            return attr_name
    raise AttributeError('access denied')


class EstLuaExec:
    def __init__(self):
        self._loaded_scripts = {}

        self.runtime = lupa.LuaRuntime(register_eval=False, attribute_filter=filter_attribute_access)
        self.est_context = LuaEstEngineContext()

    def load_script(self, string_name):
        path = _SCRIPT_LOC + string_name
        if not path.endswith('.lua'):
            path = path + '.lua'
        lua_script_syntax = _load_luaexec_file(path)

        func = self.runtime.eval(lua_script_syntax)
        self._loaded_scripts[string_name] = func

        return True

    def exec_script(self, string_name, **kwargs):
        lua_script = self._loaded_scripts[string_name]

        resp = lua_script(kwargs, self.est_context)

def LuaEstEngineContext():
    return {
        "load_ent": __fun_load_ent,
        "save_ent": __fun_save_ent,
    }

def _load_luaexec_file(path):
    with open(path) as fh:
        lua_script = fh.read()
    return lua_script


def __fun_load_ent(wid, sid):
    with open(_DAL_LOC+wid+'_'+sid+'.json') as fh:
        ent = json.load(fh)
    return ent

def __fun_save_ent(ent):
    with open(_DAL_LOC+ent['wid']+'_'+ent['sid']+'.json', 'w') as fh:
        json.dump(ent, fh, indent=4, sort_keys=True)
    return True
