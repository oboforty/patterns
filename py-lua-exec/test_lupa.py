import luaexec


lua = luaexec.EstLuaExec()
lua.load_script("myscript.lua")

lua.exec_script("myscript.lua", in_gold=5)
