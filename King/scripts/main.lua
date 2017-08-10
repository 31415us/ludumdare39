data = {}
data.people = {
    willy = {
        on_new_day = function()
            Log("willy new day")
        end,
    },
    daisy = {
        on_new_day = function()
            Log("daisy new day")
        end,
    }
}

local king = require "king"

data.day = 1
data.king.health = 20
data.king.max_health = 20

local tmp = {
    name = "Bob the farmer",
    on_receive = function()
        SetChoices({
            text = "My lord, my deepest thanks for taking the time...",
            choices = {
                {
                    "goodbye",
                    king.leave_conversation,
                }
            },
        })
    end,
    waiting = true,
}

local farmer = {
    name = "Bob the farmer",
    on_receive = function()
        SetChoices({
            text = "My lord, my deepest thanks for taking the time...",
            choices = {
                {
                    "goodbye",
                    king.leave_conversation,
                }
            },
        })
    end,
    on_reject = function()
        Log("Bob the farmer was rejected...")
    end,
    waiting = true,
}

data.king.waiting = {
    {
        name = "Sir Jamie Oliver",
        on_receive = function()
            SetChoices({
                text = "Hi father...",
                choices = {
                    {
                        "goodbye",
                        king.leave_conversation,
                    }
                },
            })
        end,
        on_reject = function()
            Log("Sir Jamie Oliver was rejected...")
        end,
        waiting = true,
    },
    {
        name = "Bob the farmer",
        on_receive = function()
            SetChoices({
                text = "My lord, my deepest thanks for taking the time...",
                choices = {
                    {
                        "goodbye",
                        king.leave_conversation,
                    }
                },
            })
        end,
        on_reject = function()
            Log("Bob the farmer was rejected...")
        end,
        waiting = true,
    },
    farmer,
    farmer,
    farmer,
    farmer,
    farmer,
    farmer,
}

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

SetChoices(king.make_new_day())
