function(params, est)

    local ent = est.load_ent("wid1", "myent")

    ent["iso"] = "HU"
    ent["items"]["gold"] = ent["items"]["gold"] + params["in_gold"]
    ent["lvl"] = ent["lvl"] + 1

    est.save_ent(ent)
end
