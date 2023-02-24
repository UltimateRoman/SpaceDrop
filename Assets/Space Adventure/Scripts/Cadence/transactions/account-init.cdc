import SpaceDropNFT from 0xf3fcd2c1a78f5eee
import SampleToken from 0xf3fcd2c1a78f5eee

// This transaction initialises a new account by creating an empty
// NFT collection and an empty Token Vault, and creating the 
// required capabilities for them. New accounts must sign and submit
// this transaction before they can use the FlowSDK Sample NFTs or Tokens. 

transaction {

    prepare(acct: AuthAccount) {
        // // store an empty NFT Collection in account storage
        // acct.save(<-SpaceDropNFT.createEmptyCollection(), to: SpaceDropNFT.CollectionStoragePath)

        // publish a reference to the Collection in storage
        acct.link<&{SpaceDropNFT.NFTReceiver}>(SpaceDropNFT.CollectionPublicPath, target: SpaceDropNFT.CollectionStoragePath)

        // // create a new empty vault instance
        // let vaultA <- SampleToken.createEmptyVault()

        // // Store the vault in the account storage
        // acct.save<@SampleToken.Vault>(<-vaultA, to: SampleToken.VaultStoragePath)

        // Create a public Receiver capability to the Vault
        let ReceiverRef = acct.link<&SampleToken.Vault{SampleToken.Receiver, SampleToken.Balance}>(SampleToken.VaultPublicPath, target: SampleToken.VaultStoragePath)
    }
}
