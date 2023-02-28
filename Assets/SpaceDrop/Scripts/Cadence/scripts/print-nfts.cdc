import SpaceDropNFT from 0xf8d6e0586b0a20c7

// Print the NFTs owned by account.
pub fun main(account: Address): [UInt64] {
    // Get the public account object
    let nftOwner = getAccount(account)

    // Find the public Receiver capability for their Collection
    let capability = nftOwner.getCapability<&{SpaceDropNFT.NFTReceiver}>(SpaceDropNFT.CollectionPublicPath)

    // borrow a reference from the capability
    let receiverRef = capability.borrow()
        ?? panic("Could not borrow the receiver reference")

    return receiverRef.getIDs()
}
