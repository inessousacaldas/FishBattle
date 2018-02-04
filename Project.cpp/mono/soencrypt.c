#include <fcntl.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <unistd.h>


/* 32-bit ELF base types. */
typedef unsigned int Elf32_Addr;
typedef unsigned short Elf32_Half;
typedef unsigned int Elf32_Off;
typedef signed int Elf32_Sword;
typedef unsigned int Elf32_Word;

#define EI_NIDENT 16

/*
 * ELF header.
 */

typedef struct {
    unsigned char e_ident[EI_NIDENT];
    /* File identification. */
    Elf32_Half e_type;
    /* File type. */
    Elf32_Half e_machine;
    /* Machine architecture. */
    Elf32_Word e_version;
    /* ELF format version. */
    Elf32_Addr e_entry;
    /* Entry point. */
    Elf32_Off e_phoff;
    /* Program header file offset. */
    Elf32_Off e_shoff;
    /* Section header file offset. */
    Elf32_Word e_flags;
    /* Architecture-specific flags. */
    Elf32_Half e_ehsize;
    /* Size of ELF header in bytes. */
    Elf32_Half e_phentsize;
    /* Size of program header entry. */
    Elf32_Half e_phnum;
    /* Number of program header entries. */
    Elf32_Half e_shentsize;
    /* Size of section header entry. */
    Elf32_Half e_shnum;
    /* Number of section header entries. */
    Elf32_Half e_shstrndx;  /* Section name strings section. */
} Elf32_Ehdr;

/*
 * Section header.
 */

typedef struct {
    Elf32_Word sh_name;
    /* Section name (index into the
               section header string table). */
    Elf32_Word sh_type;
    /* Section type. */
    Elf32_Word sh_flags;
    /* Section flags. */
    Elf32_Addr sh_addr;
    /* Address in memory image. */
    Elf32_Off sh_offset;
    /* Offset in file. */
    Elf32_Word sh_size;
    /* Size in bytes. */
    Elf32_Word sh_link;
    /* Index of a related section. */
    Elf32_Word sh_info;
    /* Depends on section type. */
    Elf32_Word sh_addralign;
    /* Alignment in bytes. */
    Elf32_Word sh_entsize; /* Size of each entry in section. */
} Elf32_Shdr;


int main(int argc, char **argv) {
    char target_section[] = ".cilu_magic_text";
    char *shstr = NULL;
    char *content = NULL;
    Elf32_Ehdr ehdr;
    Elf32_Shdr shdr;
    int i;
    unsigned int base, length;
    unsigned short nblock;
    unsigned short nsize;
    unsigned char block_size = 16;

    int fd;

    if (argc < 2) {
        puts("Input .so file");
        return -1;
    }

    fd = open(argv[1], O_RDWR);
    if (fd < 0) {
        printf("open %s failed\n", argv[1]);
        goto _error;
    }

    if (read(fd, &ehdr, sizeof(Elf32_Ehdr)) != sizeof(Elf32_Ehdr)) {
        puts("Read ELF header error");
        goto _error;
    }

    lseek(fd, ehdr.e_shoff + sizeof(Elf32_Shdr) * ehdr.e_shstrndx, SEEK_SET);

    if (read(fd, &shdr, sizeof(Elf32_Shdr)) != sizeof(Elf32_Shdr)) {
        puts("Read ELF section string table error");
        goto _error;
    }

    if ((shstr = (char *) malloc(shdr.sh_size)) == NULL) {
        puts("Malloc space for section string table failed");
        goto _error;
    }

    lseek(fd, shdr.sh_offset, SEEK_SET);
    if (read(fd, shstr, shdr.sh_size) != shdr.sh_size) {
        puts("Read string table failed");
        goto _error;
    }

    lseek(fd, ehdr.e_shoff, SEEK_SET);
    for (i = 0; i < ehdr.e_shnum; i++) {
        if (read(fd, &shdr, sizeof(Elf32_Shdr)) != sizeof(Elf32_Shdr)) {
            puts("Find section .text procedure failed");
            goto _error;
        }
        if (strcmp(shstr + shdr.sh_name, target_section) == 0) {
            base = shdr.sh_offset;
            length = shdr.sh_size;
            printf("Find section %s\n", target_section);
            break;
        }
    }

    lseek(fd, base, SEEK_SET);
    content = (char *) malloc(length);
    if (content == NULL) {
        puts("Malloc space for content failed");
        goto _error;
    }
    if (read(fd, content, length) != length) {
        puts("Read section .text failed");
        goto _error;
    }

    nblock = length / block_size;
    nsize = base / 4096 + (base % 4096 == 0 ? 0 : 1);
    printf("base = %d, length = %d\n", base, length);
    printf("nblock = %d, nsize = %d\n", nblock, nsize);

    ehdr.e_entry = (length << 16) + nsize;
    ehdr.e_shoff = base;


    for (i = 0; i < length; i++) {
        content[i] = ~content[i];
    }


    lseek(fd, 0, SEEK_SET);
    if (write(fd, &ehdr, sizeof(Elf32_Ehdr)) != sizeof(Elf32_Ehdr)) {
        puts("Write ELFhead to .so failed");
        goto _error;
    }


    lseek(fd, base, SEEK_SET);
    if (write(fd, content, length) != length) {
        puts("Write modified content to .so failed");
        goto _error;
    }

    puts("Completed");
    _error:
    free(content);
    free(shstr);
    close(fd);
    return 0;
}