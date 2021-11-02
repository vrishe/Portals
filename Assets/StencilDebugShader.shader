Shader "Stencil/Debug"
{
    Properties
    {
    }

    SubShader
    {
        ZTest  Always
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Tags { 
            "RenderType" = "Debug"
            "Queue"      = "Transparent"
        }

        Pass {
            Stencil {
                Ref 0
                Comp Equal
            }

            Color(1.000, 0.388, 0.063, 0.3)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 1
                Comp Equal
            }

            Color (0.102, 0.278, 0.435, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 2
                Comp Equal
            }

            Color(0.333, 0.459, 0.184, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 4
                Comp Equal
            }

            Color(1.000, 0.824, 0.000, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 8
                Comp Equal
            }

            Color(0.576, 0.553, 0.824, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 16
                Comp Equal
            }

            Color(0.176, 0.427, 0.400, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 32
                Comp Equal
            }

            Color(0.749, 0.631, 0.612, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 64
                Comp Equal
            }

            Color(0.757, 0.020, 0.204, 0.5)
        }

        Pass {
            Stencil {
                Ref 255
                ReadMask 128
                Comp Equal
            }

            Color(0.851, 0.902, 0.922, 0.5)
        }
    }
}
