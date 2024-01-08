"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var comps_1 = require("onejs/comps");
var color_parser_1 = require("onejs/utils/color-parser");
var preact_1 = require("preact");
var hooks_1 = require("preact/hooks");
var App = function () {
    var _a = (0, hooks_1.useState)(true), show1 = _a[0], setShow1 = _a[1];
    var _b = (0, hooks_1.useState)(true), show2 = _b[0], setShow2 = _b[1];
    return (0, preact_1.h)("gradientrect", { class: "w-full h-full flex-row justify-center items-center", colors: [(0, color_parser_1.parseColor)("#42c873"), (0, color_parser_1.parseColor)("#06a0bb")] },
        (0, preact_1.h)("div", { class: "w-64 h-64 justify-center items-center" },
            (0, preact_1.h)("div", { class: "h-32 w-32 mb-8" },
                (0, preact_1.h)(comps_1.Transition, { class: "h-full w-full", show: show1, appear: true, enter: "transition[opacity,rotate,scale] duration-[400ms]", enterFrom: "opacity-0 rotate-[-120deg] scale-50", enterTo: "opacity-100 rotate-0 scale-100", leave: "transition[opacity,rotate,scale] duration-200 ease-in-out", leaveFrom: "opacity-100 rotate-0 scale-100", leaveTo: "opacity-0 scale-75" },
                    (0, preact_1.h)("div", { class: "h-full w-full rounded-md bg-orange-200" }))),
            (0, preact_1.h)("button", { onClick: function () { return setShow1(function (show1) { return !show1; }); }, text: "Toggle" })),
        (0, preact_1.h)("div", { class: "w-64 h-64 justify-center items-center" },
            (0, preact_1.h)("div", { class: "h-32 w-32 mb-8" },
                (0, preact_1.h)(comps_1.Transition, { class: "h-full w-full flex-row justify-around", show: show2, appear: true },
                    (0, preact_1.h)("div", { class: "h-full w-[48%]" },
                        (0, preact_1.h)(comps_1.Transition.Child, { class: "h-full w-full", enter: "transition[translate,opacity] duration-[400ms]", enterFrom: "opacity-0 translate-y-[20%]", enterTo: "opacity-100 translate-y-[0]", leave: "transition[translate,opacity] duration-200 ease-in-out", leaveFrom: "opacity-100 translate-y-[0]", leaveTo: "opacity-0 translate-y-[20%]" },
                            (0, preact_1.h)("div", { class: "h-full w-full rounded-md bg-orange-200" }))),
                    (0, preact_1.h)("div", { class: "h-full w-[48%]" },
                        (0, preact_1.h)(comps_1.Transition.Child, { class: "h-full w-full", enter: "transition[translate,opacity] duration-[400ms]", enterFrom: "opacity-0 translate-y-[-20%]", enterTo: "opacity-100 translate-y-[0]", leave: "transition[translate,opacity] duration-200 ease-in-out", leaveFrom: "opacity-100 translate-y-[0]", leaveTo: "opacity-0 translate-y-[-20%]" },
                            (0, preact_1.h)("div", { class: "h-full w-full rounded-md bg-orange-200" }))))),
            (0, preact_1.h)("button", { onClick: function () { return setShow2(function (show2) { return !show2; }); }, text: "Toggle" })));
};
(0, preact_1.render)((0, preact_1.h)(App, null), document.body);
