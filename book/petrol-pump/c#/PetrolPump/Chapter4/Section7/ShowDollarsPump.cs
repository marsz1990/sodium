﻿using PetrolPump.Chapter4.Section4;
using Sodium;

namespace PetrolPump.Chapter4.Section7
{
    public class ShowDollarsPump : IPump
    {
        public Outputs Create(Inputs inputs)
        {
            LifeCycle lc = new LifeCycle(inputs.SNozzle1,
                inputs.SNozzle2,
                inputs.SNozzle3);
            Fill fi = new Fill(lc.SStart.Map(_ => Unit.Value),
                inputs.SFuelPulses, inputs.Calibration,
                inputs.Price1, inputs.Price2, inputs.Price3,
                lc.SStart);
            return new Outputs()
                .SetDelivery(lc.FillActive.Map(
                    m =>
                        m.Equals(Maybe.Just(Fuel.One)) ? Delivery.Fast1 :
                            m.Equals(Maybe.Just(Fuel.Two)) ? Delivery.Fast2 :
                                m.Equals(Maybe.Just(Fuel.Three)) ? Delivery.Fast3 :
                                    Delivery.Off))
                .SetSaleCostLcd(fi.DollarsDelivered.Map(
                    q => q.ToString("#0.00")))
                .SetSaleQuantityLcd(fi.LitersDelivered.Map(
                    q => q.ToString("#0.00")))
                .SetPriceLcd1(PriceLcd(lc.FillActive, fi.Price, Fuel.One,
                    inputs))
                .SetPriceLcd2(PriceLcd(lc.FillActive, fi.Price, Fuel.Two,
                    inputs))
                .SetPriceLcd3(PriceLcd(lc.FillActive, fi.Price, Fuel.Three,
                    inputs));
        }

        public static Cell<string> PriceLcd(
            Cell<IMaybe<Fuel>> fillActive,
            Cell<double> fillPrice,
            Fuel fuel,
            Inputs inputs)
        {
            Cell<double> idlePrice;
            switch (fuel)
            {
                case Fuel.One:
                    idlePrice = inputs.Price1;
                    break;
                case Fuel.Two:
                    idlePrice = inputs.Price2;
                    break;
                case Fuel.Three:
                    idlePrice = inputs.Price3;
                    break;
                default:
                    idlePrice = null;
                    break;
            }
            return fillActive.Lift(fillPrice, idlePrice,
                (fuelSelectedMaybe, fillPriceLocal, idlePriceLocal) =>
                    fuelSelectedMaybe.Match(f => f == fuel ? fillPriceLocal.ToString("#0.000") : string.Empty, () => idlePriceLocal.ToString("#0.000")));
        }
    }
}