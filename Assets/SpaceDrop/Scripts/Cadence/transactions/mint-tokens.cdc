import SampleToken from 0xf8d6e0586b0a20c7

transaction(acc: Address, amount: UFix64) {

  // Public Vault Receiver References for both accounts
  let acctCapability: Capability<&AnyResource{SampleToken.Receiver}>

  // Private minter references for this account to mint tokens
  let minterRef: &SampleToken.VaultMinter

  prepare(acct: AuthAccount) {
    
    let account = getAccount(acc)

    // Retrieve public Vault Receiver references for both accounts
    self.acctCapability = account.getCapability<&AnyResource{SampleToken.Receiver}>(SampleToken.VaultPublicPath)

    // Get the stored Minter reference
    self.minterRef = acct.borrow<&SampleToken.VaultMinter>(from: SampleToken.MinterStoragePath)
        ?? panic("Could not borrow owner's vault minter reference")
  }

  execute {
    // Mint tokens for both accounts
    self.minterRef.mintTokens(amount: amount, recipient: self.acctCapability)
  }
}
