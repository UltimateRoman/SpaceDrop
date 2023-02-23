import NonFungibleToken from "../../contracts/utility/NonFungibleToken.cdc"
import SpaceDropNFT from "../../contracts/SpaceDropNFT.cdc"

pub fun main(address: Address): Int {
    let account = getAccount(address)

    let collectionRef = account
        .getCapability(SpaceDropNFT.CollectionPublicPath)
        .borrow<&{NonFungibleToken.CollectionPublic}>()
        ?? panic("Could not borrow capability from public collection")
    
    return collectionRef.getIDs().length
}