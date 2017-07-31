require "Stack"
require "math"

data.king = {
    convesation_stack = {},
    waiting = {},
    current_audience = -1,
}

local health_texts = {
    "I feel horrible... I think I'm not going to make it much longer",
    "Tonight I coughed bood... ",
    "It's getting worse... ",
    "I feel so powerless...",
    "I feel so old...",
}

local get_health_text = function()
    local num = math.ceil((data.king.health / data.king.max_health) * (#health_texts - 0.01))
    return "(" .. data.king.health .. ") " .. health_texts[num]
end

local you_died = {
    text = "you died!",
    choices = {},
}

local update_waiting = function()
    local res = {}
    local waiting = data.king.waiting
    for i = 1, #waiting do
        if (waiting[i].waiting) then
            res[#res + 1] = waiting[i]
        end
    end
    data.king.waiting = res
end

local make_waiting_string = function()
    local res = ""
    local waiting = data.king.waiting
    if (#waiting > 1) then
        res = waiting[1].name
        for i = 2, #waiting - 1 do
            res = res .. ", " .. waiting[i].name
        end
        return res .. " and " .. waiting[#waiting].name .. " are waiting for an audience..."
    elseif (#waiting == 1) then
        return waiting[1].name .. " is waiting for an audience..."
    else
        return "No one is waiting for an audience..."
    end
end

local this = {}

this.leave_conversation = function()
    data.king.waiting[data.king.current_audience].waiting = false
    data.king.current_audience = -1
    SetChoices(Stack.pop(data.king.convesation_stack)())
end
    
this.push_conversation = function(fn)
    Stack.push(data.king.convesation_stack, fn)
end

this.make_send_in = function()
    local text = "Who would you like to receive?"
    local waiting = data.king.waiting
    local choices = {}
    for i = 1, #waiting do
        choices[#choices + 1] = {
            waiting[i].name,
            function()
                data.king.current_audience = i
                waiting[i].on_receive()
            end,
        }
    end
    choices[#choices + 1] = {
        "<back>",
        function()
            SetChoices(Stack.pop(data.king.convesation_stack)())
        end,
    }
    return {
        text = text,
        choices = choices,
    }
end

this.make_new_day = function()
    local text = "Day " .. data.day .. ":\n"
    text = text .. get_health_text() .. "\n"
    return {
        text = text,
        choices = {
            {
                "Let's work...",
                function()
                    SetChoices(this.make_good_morning())
                end,
            }
        }
    }
end

this.make_done_choice = function()
    return {
        "I'm done for today...",
        function ()
            Log("Going to bed...")
            local waiting = data.king.waiting
            for i = 1, #waiting do
                local rejectFn = waiting[i].on_reject
                if (rejectFn ~= nil) then rejectFn() end
            end
            data.day = data.day + 1
            data.king.health = data.king.health - 1
            if (data.king.health <= 0) then
                SetChoices(you_died)
            else
                SetChoices(this.make_new_day())
            end
        end,
    }
end

this.make_good_morning = function()
    update_waiting()
    local text = "Good morning your majesty!"
    text = text .. "\n" .. make_waiting_string()
    local choices = {}
    local waiting = data.king.waiting
    if (#waiting > 0) then
        choices = {
            {
                "send in ...",
                function()
                    this.push_conversation(this.make_good_morning)
                    SetChoices(this.make_send_in())
                end
            },
            this.make_done_choice(),
        }
    else
        choices = {
            this.make_done_choice(),
        }
    end
    return {
        text = text,
        choices = choices,
    }
end

return {
    make_new_day = this.make_new_day,
    leave_conversation = this.leave_conversation,
    push_conversation = this.push_conversation,
}