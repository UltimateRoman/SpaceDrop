import SpaceDropNFT from "../../contracts/SpaceDropNFT.cdc"

pub fun main(): UInt64 {
    return SpaceDropNFT.totalSupply
}