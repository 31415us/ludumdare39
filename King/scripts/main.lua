
data = {}

local servant = require "servant"

--[[
local pauls_cows = {
    text = 'Your majesty! My cows are sick.',
    choices = {
        {
            'Go away, you filthy peasant!',
            function ()
                Log('Paul the peasant is sad.')
                -- make paul sad
            end,
        },
        {
            'I can\'t help you. Here take some money.',
            function ()
                Log('Paul the peasant is getting drunk tonight.')
                -- make paul a little less sad
            end,
        },
    }
}
]]--

SetChoices(servant.start_day())
