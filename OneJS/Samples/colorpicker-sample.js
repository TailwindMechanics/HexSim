"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Utils_1 = require("OneJS/Utils");
var UnityEngine_1 = require("UnityEngine");
var comps_1 = require("onejs/comps");
var color_parser_1 = require("onejs/utils/color-parser");
var preact_1 = require("preact");
var hooks_1 = require("preact/hooks");
var texture = new UnityEngine_1.Texture2D(200, 200, UnityEngine_1.TextureFormat.RGBA32, false);
var colors = texture.GetRawDataColor32();
var App = function () {
    (0, hooks_1.useEffect)(function () {
        onChange(0);
    }, []);
    function onChange(v) {
        var hueColor = UnityEngine_1.Color.HSVToRGB(v, 1, 1);
        Utils_1.GradientTextureFillJob.Run(colors, 200, 200, hueColor);
        texture.Apply();
    }
    return (0, preact_1.h)("gradientrect", { class: "w-full h-full justify-center items-center", colors: [(0, color_parser_1.parseColor)("#42c873"), (0, color_parser_1.parseColor)("#06a0bb")] },
        (0, preact_1.h)("image", { class: "mb-4", image: texture }),
        (0, preact_1.h)(comps_1.Slider, { class: "w-[200px]", onChange: onChange }));
};
(0, preact_1.render)((0, preact_1.h)(App, null), document.body);
