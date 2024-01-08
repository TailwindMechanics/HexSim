import { DiamondToggle, ExampleTabs, RadioToggle, Toggle } from "onejs/comps"
import { Select } from "onejs/comps"
import { Slider } from "onejs/comps"
import { parseColor as c } from "onejs/utils/color-parser"
import { h, render } from "preact"
import { useEffect, useState } from "preact/hooks"

const people = [
    { id: 1, name: 'Durward Reynolds' },
    { id: 2, name: 'Kenton Towne' },
    { id: 3, name: 'Therese Wunsch' },
    { id: 4, name: 'Benedict Kessler' },
    { id: 5, name: 'Katelyn Rohan' },
]

const tiers = [{
    label: 'Low',
    value: 'low',
}, {
    label: 'Medium',
    value: 'medium',
}, {
    label: 'High',
    value: 'high',
}]

const App = () => {
    const [selectedPerson, setSelectedPerson] = useState(people[0])
    const [selectedTier, setSelectedTier] = useState(tiers[0].value)
    const [checked, setChecked] = useState(false)
    const [checked2, setChecked2] = useState(false)

    useEffect(() => {
        log(`Selected person: ${selectedPerson.name}`)
    }, [selectedPerson])

    useEffect(() => {
        log(`Selected tier: ${selectedTier}`)
    }, [selectedTier])

    useEffect(() => {
        log(`Toggle: ${checked}`)
    }, [checked])

    useEffect(() => {
        log(`Toggle 2: ${checked2}`)
    }, [checked2])

    return <gradientrect class="w-full h-full justify-center items-center" colors={[c("#42c873"), c("#06a0bb")]}>
        <ExampleTabs class="mb-4" />
        <Select class="min-w-[200px] mb-4" items={people} onChange={setSelectedPerson} />
        <Toggle class="mb-4" checked={checked} onChange={setChecked} />
        <DiamondToggle class="mb-4" checked={checked2} onChange={setChecked2} />
        <RadioToggle class="mb-4" items={tiers} onChange={setSelectedTier} />
        <Slider class="mb-4 w-[300px]" />
    </gradientrect>
}

render(<App />, document.body)