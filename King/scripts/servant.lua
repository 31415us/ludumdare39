require "Queue"

data.queue = Queue.new()

local blabla = {
    text = "blabla!",
    choices = {},
}

local you_died = {
    text = "you died!",
    choices = {},
}

Queue.pushright(data.queue, {
    name = "Prince Henry",
    data = {
        text = "Hi father...",
        choices = {},
    }
})

Queue.pushright(data.queue, {
    name = "Bob the farmer",
    data = {
        text = "My lord, my deepest thanks for taking the time...",
        choices = {},
    }
})

local this = {}

this.send_in_guest = function()
    if (Queue.size(data.queue) <= 0) then Log("Error: queue empty") return you_died end
    local guest = Queue.popleft(data.queue)
    return guest.data
end

this.make_send_in_choice = function()
    return {
        "Send in " .. data.queue[data.queue.first].name .. "...",
        function ()
            Log("receiving first guest")
            SetChoices(this.send_in_guest())
        end,
    }
end

this.make_send_away_choice = function()
    return {
        "Send " .. data.queue[data.queue.first].name .. " away...",
        function ()
            Log("send away " .. data.queue[data.queue.first].name)
            Queue.popleft(data.queue)
            SetChoices(this.start_day())
        end,
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

this.start_day = function()
    local text = "Good morning your majesty!"
    local choices = {}
    if (Queue.size(data.queue) > 1) then
        local peopleString = data.queue[data.queue.first].name
        for i = data.queue.first + 1, data.queue.last - 1 do
            peopleString = peopleString .. ", " .. data.queue[i].name
        end
        peopleString = peopleString .. " and " .. data.queue[data.queue.last].name
        text = text .. "\n" .. peopleString .. " are waiting for an audience"
        choices = {
            this.make_send_in_choice(),
            this.make_send_away_choice(),
            this.make_done_choice(),
        }
    elseif (Queue.size(data.queue) == 1) then
        text = text .. "\n" .. data.queue[data.queue.first].name .. " is waiting for an audience"
        choices = {
            this.make_send_in_choice(),
            this.make_done_choice(),
        }
    else
        choices = {
            this.make_done_choice(),
        }
    end
    --text = text .. "\nHow can I be of service?"
    return {
        text = text,
        choices = choices,
    }
end


return {
    start_day = this.start_day,
}