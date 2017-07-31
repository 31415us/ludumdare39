Stack = {
    push = function(list, value)
        list[#list + 1] = value
    end,

    pop = function(list)
        if #list == 0 then error("stack is empty") end
        local res = list[#list]
        list[#list] = nil
        return res
    end,
}