require "Stack"

data.king = {
    convesation_stack = {},
    waiting = {},
    current_audience = -1,
}

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

this.make_done_choice = function()
    return {
        "I'm done for today...",
        function ()
            Log("Going to bed...")
            SetChoices(you_died)
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
    make_good_morning = this.make_good_morning,
    leave_conversation = this.leave_conversation,
    push_conversation = this.push_conversation,
}