import { Transition } from "onejs/comps"
import { parseColor as c } from "onejs/utils/color-parser"
import { h, render } from "preact"
import { useState } from "preact/hooks"

const App = () => {
    const [show1, setShow1] = useState(true)
    const [show2, setShow2] = useState(true)

    return <gradientrect class="w-full h-full flex-row justify-center items-center" colors={[c("#42c873"), c("#06a0bb")]}>
        <div class="w-64 h-64 justify-center items-center">
            <div class="h-32 w-32 mb-8">
                <Transition
                    class="h-full w-full"
                    show={show1}
                    appear={true}
                    enter="transition[opacity,rotate,scale] duration-[400ms]"
                    enterFrom="opacity-0 rotate-[-120deg] scale-50"
                    enterTo="opacity-100 rotate-0 scale-100"
                    leave="transition[opacity,rotate,scale] duration-200 ease-in-out"
                    leaveFrom="opacity-100 rotate-0 scale-100"
                    leaveTo="opacity-0 scale-75"
                >
                    <div class="h-full w-full rounded-md bg-orange-200" />
                </Transition>
            </div>
            <button onClick={() => setShow1((show1) => !show1)} text="Toggle" />
        </div>
        <div class="w-64 h-64 justify-center items-center">
            <div class="h-32 w-32 mb-8">
                <Transition class="h-full w-full flex-row justify-around" show={show2} appear={true}>
                    <div class="h-full w-[48%]">
                        <Transition.Child class="h-full w-full"
                            enter="transition[translate,opacity] duration-[400ms]"
                            enterFrom="opacity-0 translate-y-[20%]"
                            enterTo="opacity-100 translate-y-[0]"
                            leave="transition[translate,opacity] duration-200 ease-in-out"
                            leaveFrom="opacity-100 translate-y-[0]"
                            leaveTo="opacity-0 translate-y-[20%]"
                        >
                            <div class="h-full w-full rounded-md bg-orange-200" />
                        </Transition.Child>
                    </div>
                    <div class="h-full w-[48%]">
                        <Transition.Child class="h-full w-full"
                            enter="transition[translate,opacity] duration-[400ms]"
                            enterFrom="opacity-0 translate-y-[-20%]"
                            enterTo="opacity-100 translate-y-[0]"
                            leave="transition[translate,opacity] duration-200 ease-in-out"
                            leaveFrom="opacity-100 translate-y-[0]"
                            leaveTo="opacity-0 translate-y-[-20%]"
                        >
                            <div class="h-full w-full rounded-md bg-orange-200" />
                        </Transition.Child>
                    </div>
                </Transition>
            </div>
            <button onClick={() => setShow2((show2) => !show2)} text="Toggle" />
        </div>
    </gradientrect>
}

render(<App />, document.body)