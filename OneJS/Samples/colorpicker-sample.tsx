import { GradientTextureFillJob } from "OneJS/Utils"
import { Color, Texture2D, TextureFormat } from "UnityEngine"
import { Slider } from "onejs/comps"
import { parseColor as c } from "onejs/utils/color-parser"
import { h, render } from "preact"
import { useEffect } from "preact/hooks"

var texture = new Texture2D(200, 200, TextureFormat.RGBA32, false)
var colors = texture.GetRawDataColor32()

const App = () => {

    useEffect(() => {
        onChange(0)
    }, [])

    function onChange(v: number) {
        var hueColor = Color.HSVToRGB(v, 1, 1)
        GradientTextureFillJob.Run(colors, 200, 200, hueColor)
        texture.Apply()
    }

    return <gradientrect class="w-full h-full justify-center items-center" colors={[c("#42c873"), c("#06a0bb")]}>
        <image class="mb-4" image={texture} />
        <Slider class="w-[200px]" onChange={onChange} />
    </gradientrect>
}

render(<App />, document.body)