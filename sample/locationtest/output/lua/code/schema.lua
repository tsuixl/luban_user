
--[[------------------------------------------------------------------------------
-- <auto-generated>
--     This code was generated by a tool.
--     Changes to this file may cause incorrect behavior and will be lost if
--     the code is regenerated.
-- </auto-generated>
--]]------------------------------------------------------------------------------

local setmetatable = setmetatable
local pairs = pairs
local ipairs = ipairs
local tinsert = table.insert

local function SimpleClass()
    local class = {}
    class.__index = class
    class.New = function(...)
        local ctor = class.ctor
        local o = ctor and ctor(...) or {}
        setmetatable(o, class)
        return o
    end
    return class
end


local function get_map_size(m)
    local n = 0
    for _ in pairs(m) do
        n = n + 1
    end
    return n
end

local enums =
{
    ---@class item.EQuality
     ---@field public WHITE integer @最差品质
     ---@field public BLUE integer @蓝色的
     ---@field public PURPLE integer @紫色的
     ---@field public RED integer @最高品质
    ['item.EQuality'] = {   WHITE=1,  BLUE=2,  PURPLE=3,  RED=4,  };
    ---@class test.AccessFlag
     ---@field public WRITE integer
     ---@field public READ integer
     ---@field public TRUNCATE integer
     ---@field public NEW integer
     ---@field public READ_WRITE integer @位标记使用示例
    ['test.AccessFlag'] = {   WRITE=1,  READ=2,  TRUNCATE=4,  NEW=8,  READ_WRITE=3,  };
}

local tables =
{
    { name='TestConfig', file='Test/TestConfig', mode='map', index='id', value_type='TestItem' },
    { name='TestConfig2', file='Test/TestConfig2', mode='map', index='id', value_type='TestItem2' },
    { name='TestConfig3', file='Test/TestConfig3', mode='map', index='id', value_type='TestItem3' },
}

local function InitTypes(methods)
    local readBool = methods.readBool
    local readByte = methods.readByte
    local readShort = methods.readShort
    local readFshort = methods.readFshort
    local readInt = methods.readInt
    local readFint = methods.readFint
    local readLong = methods.readLong
    local readFlong = methods.readFlong
    local readFloat = methods.readFloat
    local readDouble = methods.readDouble
    local readSize = methods.readSize

    local readString = methods.readString

    local function readList(bs, keyFun)
        local list = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(list, keyFun(bs))
        end
        return list
    end

    local readArray = readList

    local function readSet(bs, keyFun)
        local set = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(set, keyFun(bs))
        end
        return set
    end

    local function readMap(bs, keyFun, valueFun)
        local map = {}
        for i = 1, readSize(bs) do
            local k = keyFun(bs)
            local v = valueFun(bs)
            map[k] = v
        end
        return map
    end

    local function readNullableBool(bs)
        if readBool(bs) then
            return readBool(bs)
        end
    end
    
    local beans = {}
        do
        ---@class TestItem 
         ---@field public id integer @这是id
         ---@field public name string @名字
         ---@field public desc string @描述
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
            local class = {
                { name='id', type='integer'},
                { name='name', type='string'},
                { name='desc', type='string'},
                { name='locationText', type='LocationText'},
                { name='desc3', type='string'},
                { name='descList', type='LocationText[]'},
                { name='descList2', type='string[]'},
            }
            beans['TestItem'] = class
        end
        do
        ---@class LocationText 
         ---@field public zh string
         ---@field public en string
            local class = {
                { name='zh', type='string'},
                { name='en', type='string'},
            }
            beans['LocationText'] = class
        end
        do
        ---@class TestItem2 
         ---@field public id integer @这是id
         ---@field public name string @名字
         ---@field public desc string @描述
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
            local class = {
                { name='id', type='integer'},
                { name='name', type='string'},
                { name='desc', type='string'},
                { name='locationText', type='LocationText'},
                { name='desc3', type='string'},
                { name='descList', type='LocationText[]'},
                { name='descList2', type='string[]'},
            }
            beans['TestItem2'] = class
        end
        do
        ---@class TestItem3 
         ---@field public id integer @这是id
         ---@field public name string @名字
         ---@field public desc string @描述
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
            local class = {
                { name='id', type='integer'},
                { name='name', type='string'},
                { name='desc', type='string'},
                { name='locationText', type='LocationText'},
                { name='desc3', type='string'},
                { name='descList', type='LocationText[]'},
                { name='descList2', type='string[]'},
            }
            beans['TestItem3'] = class
        end
    
    local beans = {}
    do
    ---@class TestItem 
         ---@field public id integer
         ---@field public name string
         ---@field public desc string
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
        local class = SimpleClass()
        class._id = -1082458395
        class._type_ = 'TestItem'
        local id2name = {  }
        class._deserialize = function(bs)
            local o = {
			self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)

			self.name = readText(bs, textList)
			
			self.name = readText(bs, textList)
			
			self.name = readText(bs, textList)

			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)

			self.locationText = beans.LocationText.__deserialize(bs, textList)
			
			beans.LocationText.__reDeserializeText(self.locationText, bs, textList)
			
			beans.LocationText.__reDeserializeTextLua(self.locationText, bs, textList)

			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)

			
			do
			    self.descList = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = beans.LocationText.__deserialize(bs, textList)
			        tinsert(self.descList, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeText(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeTextLua(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end

			
			do
			    self.descList2 = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = readText(bs, textList)
			        tinsert(self.descList2, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end

            }
            setmetatable(o, class)
            return o
        end
        beans[class._type_] = class
    end
    do
    ---@class LocationText 
         ---@field public zh string
         ---@field public en string
        local class = SimpleClass()
        class._id = 106023586
        class._type_ = 'LocationText'
        local id2name = {  }
        class._deserialize = function(bs)
            local o = {
			self.zh = readText(bs, textList)
			
			self.zh = readText(bs, textList)
			
			self.zh = readText(bs, textList)

			self.en = readString(bs, textList)
			
			--self.en = readString(bs, textList)
			
			--self.en = readString(bs, textList)

            }
            setmetatable(o, class)
            return o
        end
        beans[class._type_] = class
    end
    do
    ---@class TestItem2 
         ---@field public id integer
         ---@field public name string
         ---@field public desc string
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
        local class = SimpleClass()
        class._id = 803528173
        class._type_ = 'TestItem2'
        local id2name = {  }
        class._deserialize = function(bs)
            local o = {
			self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)

			self.name = readString(bs, textList)
			
			--self.name = readString(bs, textList)
			
			--self.name = readString(bs, textList)

			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)

			self.locationText = beans.LocationText.__deserialize(bs, textList)
			
			beans.LocationText.__reDeserializeText(self.locationText, bs, textList)
			
			beans.LocationText.__reDeserializeTextLua(self.locationText, bs, textList)

			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)

			
			do
			    self.descList = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = beans.LocationText.__deserialize(bs, textList)
			        tinsert(self.descList, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeText(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeTextLua(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end

			
			do
			    self.descList2 = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = readText(bs, textList)
			        tinsert(self.descList2, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end

            }
            setmetatable(o, class)
            return o
        end
        beans[class._type_] = class
    end
    do
    ---@class TestItem3 
         ---@field public id integer
         ---@field public name string
         ---@field public desc string
         ---@field public locationText LocationText
         ---@field public desc3 string
         ---@field public descList LocationText[]
         ---@field public descList2 string[]
        local class = SimpleClass()
        class._id = 803528174
        class._type_ = 'TestItem3'
        local id2name = {  }
        class._deserialize = function(bs)
            local o = {
			self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)
			
			--self.id = readInt(bs, textList)

			self.name = readText(bs, textList)
			
			self.name = readText(bs, textList)
			
			self.name = readText(bs, textList)

			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)
			
			self.desc = readText(bs, textList)

			self.locationText = beans.LocationText.__deserialize(bs, textList)
			
			beans.LocationText.__reDeserializeText(self.locationText, bs, textList)
			
			beans.LocationText.__reDeserializeTextLua(self.locationText, bs, textList)

			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)
			
			self.desc3 = readText(bs, textList)

			
			do
			    self.descList = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = beans.LocationText.__deserialize(bs, textList)
			        tinsert(self.descList, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeText(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList
			    for _i0=1, _n0 do
			        local _e0 = self.descList[_i0]
			        beans.LocationText.__reDeserializeTextLua(_e0, bs, textList)
			        --self.descList[_i0] = _i0
			    end
			end

			
			do
			    self.descList2 = {}
			    local _n0 = readSize()
			    for _i0=1, _n0 do
			        local _e0
			        _e0 = readText(bs, textList)
			        tinsert(self.descList2, _e0)
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end
			
			
			do
			    local _n0 = #self.descList2
			    for _i0=1, _n0 do
			        local _e0 = nil
			        _e0 = readText(bs, textList)
			        self.descList2[_i0] = _i0
			    end
			end

            }
            setmetatable(o, class)
            return o
        end
        beans[class._type_] = class
    end


    return { enums = enums, beans = beans, tables = tables }
    end

return { InitTypes = InitTypes }


