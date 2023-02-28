import SpaceDropNFT from 0xf8d6e0586b0a20c7

transaction(acc: Address) {

    // The reference to the Minter resource stored in account storage
    let minterRef: &SpaceDropNFT.NFTMinter

    prepare(acct: AuthAccount) {

        // Borrow a capability for the NFTMinter in storage
        self.minterRef = acct.borrow<&SpaceDropNFT.NFTMinter>(from: SpaceDropNFT.MinterStoragePath)
            ?? panic("could not borrow minter reference")
    }

    execute {

        // Get the recipient's public account object
        let recipient = getAccount(acc)

        // Get the Collection reference for the receiver
        // getting the public capability and borrowing a reference from it
        let receiverRef = recipient.getCapability(SpaceDropNFT.CollectionPublicPath)
                                    .borrow<&{SpaceDropNFT.NFTReceiver}>()
                                    ?? panic("Could not borrow nft receiver reference")

        // Use the minter reference to mint an NFT, which deposits
        // the NFT into the collection that is sent as a parameter.
        let newNFT <- self.minterRef.mintNFT()
        receiverRef.deposit(token: <-newNFT)
    }
}
