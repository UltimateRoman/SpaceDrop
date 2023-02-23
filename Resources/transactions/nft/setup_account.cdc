import NonFungibleToken from "../contracts/utility/NonFungibleToken.cdc"
import SpaceDropNFT from "../contracts/SpaceDropNFT.cdc"
import MetadataViews from "../contracts/utility/MetadataViews.cdc"

/// This transaction is what an account would run
/// to set itself up to receive NFTs

transaction {

    prepare(signer: AuthAccount) {
        // Return early if the account already has a collection
        if signer.borrow<&SpaceDropNFT.Collection>(from: SpaceDropNFT.CollectionStoragePath) != nil {
            return
        }

        // Create a new empty collection
        let collection <- SpaceDropNFT.createEmptyCollection()

        // save it to the account
        signer.save(<-collection, to: SpaceDropNFT.CollectionStoragePath)

        // create a public capability for the collection
        signer.link<&{NonFungibleToken.CollectionPublic, SpaceDropNFT.SpaceDropNFTCollectionPublic, MetadataViews.ResolverCollection}>(
            SpaceDropNFT.CollectionPublicPath,
            target: SpaceDropNFT.CollectionStoragePath
        )
    }
}