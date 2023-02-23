// This transaction is a template for a transaction to allow 
// anyone to add a Vault resource to their account so that 
// they can use the SampleToken

import FungibleToken from "../../contracts/utility/FungibleToken.cdc"
import SampleToken from "../../contracts/SampleToken.cdc"
import MetadataViews from "../../contracts/utility/MetadataViews.cdc"

transaction () {

    prepare(signer: AuthAccount) {

        // Return early if the account already stores a SampleToken Vault
        if signer.borrow<&SampleToken.Vault>(from: SampleToken.VaultStoragePath) != nil {
            return
        }

        // Create a new SampleToken Vault and put it in storage
        signer.save(
            <-SampleToken.createEmptyVault(),
            to: SampleToken.VaultStoragePath
        )

        // Create a public capability to the Vault that only exposes
        // the deposit function through the Receiver interface
        signer.link<&SampleToken.Vault{FungibleToken.Receiver}>(
            SampleToken.ReceiverPublicPath,
            target: SampleToken.VaultStoragePath
        )

        // Create a public capability to the Vault that exposes the Balance and Resolver interfaces
        signer.link<&SampleToken.Vault{FungibleToken.Balance, MetadataViews.Resolver}>(
            SampleToken.VaultPublicPath,
            target: SampleToken.VaultStoragePath
        )
    }
}