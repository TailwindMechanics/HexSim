"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var comps_1 = require("onejs/comps");
var comps_2 = require("onejs/comps");
var comps_3 = require("onejs/comps");
var color_parser_1 = require("onejs/utils/color-parser");
var preact_1 = require("preact");
var hooks_1 = require("preact/hooks");
var people = [
    { id: 1, name: 'Durward Reynolds' },
    { id: 2, name: 'Kenton Towne' },
    { id: 3, name: 'Therese Wunsch' },
    { id: 4, name: 'Benedict Kessler' },
    { id: 5, name: 'Katelyn Rohan' },
];
var tiers = [{
        label: 'Low',
        value: 'low',
    }, {
        label: 'Medium',
        value: 'medium',
    }, {
        label: 'High',
        value: 'high',
    }];
var App = function () {
    var _a = (0, hooks_1.useState)(people[0]), selectedPerson = _a[0], setSelectedPerson = _a[1];
    var _b = (0, hooks_1.useState)(tiers[0].value), selectedTier = _b[0], setSelectedTier = _b[1];
    var _c = (0, hooks_1.useState)(false), checked = _c[0], setChecked = _c[1];
    var _d = (0, hooks_1.useState)(false), checked2 = _d[0], setChecked2 = _d[1];
    (0, hooks_1.useEffect)(function () {
        log("Selected person: ".concat(selectedPerson.name));
    }, [selectedPerson]);
    (0, hooks_1.useEffect)(function () {
        log("Selected tier: ".concat(selectedTier));
    }, [selectedTier]);
    (0, hooks_1.useEffect)(function () {
        log("Toggle: ".concat(checked));
    }, [checked]);
    (0, hooks_1.useEffect)(function () {
        log("Toggle 2: ".concat(checked2));
    }, [checked2]);
    return (0, preact_1.h)("gradientrect", { class: "w-full h-full justify-center items-center", colors: [(0, color_parser_1.parseColor)("#42c873"), (0, color_parser_1.parseColor)("#06a0bb")] },
        (0, preact_1.h)(comps_1.ExampleTabs, { class: "mb-4" }),
        (0, preact_1.h)(comps_2.Select, { class: "min-w-[200px] mb-4", items: people, onChange: setSelectedPerson }),
        (0, preact_1.h)(comps_1.Toggle, { class: "mb-4", checked: checked, onChange: setChecked }),
        (0, preact_1.h)(comps_1.DiamondToggle, { class: "mb-4", checked: checked2, onChange: setChecked2 }),
        (0, preact_1.h)(comps_1.RadioToggle, { class: "mb-4", items: tiers, onChange: setSelectedTier }),
        (0, preact_1.h)(comps_3.Slider, { class: "mb-4 w-[300px]" }));
};
(0, preact_1.render)((0, preact_1.h)(App, null), document.body);
