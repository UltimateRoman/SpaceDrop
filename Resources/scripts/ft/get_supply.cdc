import SampleToken from "../../contracts/SampleToken.cdc"

pub fun main(): UFix64 {

    let supply = SampleToken.totalSupply

    log(supply)

    return supply
}