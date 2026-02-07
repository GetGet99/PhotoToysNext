namespace PhotoToysV2.Controls;

[QuickMarkup("""
    using Windows.Globalization.NumberFormatting;
    
    double Value = 0;
    double Minimum = 0;
    double Maximum = 0;
    double Step = 1;
    bool NumberBoxMode = false;
    <root>
        <Slider
            IsVisible=/-!NumberBoxMode-/
            Minimum=/-Minimum-/ Maximum=/-Maximum-/ StepFrequency=/-Step-/
            Value=/-Value-/ Value=>/-Value-/
        />
        <NumberBox IsVisible=/-NumberBoxMode-/ Minimum=/-Minimum-/ Maximum=/-Maximum-/ Value=/-Value-/ Value=>/-Value-/
            NumberFormatter=<DecimalFormatter
                IntegerDigits=1
                FractionDigits=/- -(int)Math.Floor(Math.Log10(Step)) -/
                NumberRounder=<IncrementNumberRounder
                    Increment=/-Step-/
                    RoundingAlgorithm=RoundHalfUp
                />
            />
        />
    </root>
    """)]
partial class NumberInput : Grid;
